var tar = require('tar');
var stream = require('stream');
var azure_storage = require('azure-storage');

module.exports = function (context, sourceArchive) {
    var blobService = azure_storage.createBlobService(process.env.DataConnection);
    var sourceStream = new stream.PassThrough();
    sourceStream.end(sourceArchive);
    sourceStream.pipe(tar.x())
        .on('entry', (entry) => {
            if (entry.type === 'File') {
                entry.pipe(
                    blobService.createWriteStreamToBlockBlob(process.env.SourceDataContainer, process.env.UnzipLocation + entry.path)
                );
            }
        })
        .on('end', () => {
            context.log('Completed unzip');
            context.bindings.outputQueueItem = `Done: ${new Date().toISOString()}`;
            context.done();
        });
};