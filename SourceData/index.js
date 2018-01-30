var tar = require('tar');
var azure_storage = require('azure-storage');

module.exports = function (context) {
    var blobService = azure_storage.createBlobService(process.env.DataConnection);
    blobService.createReadStream(process.env.SourceDataContainer, `zipped/${context.bindingData.name}`)
        .pipe(tar.x())
        .on('entry', (entry) => {
            if (entry.type === 'File') {
                entry.pipe(
                    blobService.createWriteStreamToBlockBlob(process.env.SourceDataContainer, process.env.UnzipLocation + entry.path)
                )
                .on('end', () => {
                    context.log(`Completed writing blob to: ${process.env.SourceDataContainer}/${process.env.UnzipLocation + entry.path}`);
                });
            }
        })
        .on('end', () => {
            context.log('Completed unzip');
            context.bindings.outputQueueItem = `Done: ${new Date().toISOString()}`;
            context.done();
        });
};