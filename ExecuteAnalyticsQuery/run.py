import os, requests, json

creds = (os.environ['DataBricksUsername'], os.environ['DataBricksToken'])
# Get the job_id for the specified job
jobs = requests.get('https://{0}.azuredatabricks.net/api/2.0/jobs/list'.format(os.environ['DataBricksRegion']), auth=creds).json()
job_id = [job for job in jobs['jobs'] if job['settings']['name'] == os.environ['DataBricksNotebook']][0]['job_id']

# We need to parse the connection string to get account name & key
parts = dict(part.split('=', 1) for part in os.environ['DataConnection'].split(';'))
run_job_payload = {\
    "job_id": job_id,\
    "notebook_params": {\
    "storage_account_name": parts['AccountName'], \
    "storage_account_shared_key": parts['AccountKey'], \
    "source_container": os.environ['SourceDataContainer'], \
    "source_prefix": os.environ['UnzipLocation'] + 'logs/', \
    "output_container": os.environ['AnalyticsOutputContainer'], \
    "output_prefix": os.environ['AnalyticsOutputPrefix'], 
    "done_file_path": os.environ['AnalyticsJobDone']}}

run_job_response = requests.post('https://{0}.azuredatabricks.net/api/2.0/jobs/run-now'.format(os.environ['DataBricksRegion']), auth=creds, data=json.dumps(run_job_payload)).json()
print(run_job_response)
