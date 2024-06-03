###**Live Demo:**### http://tlc-ui.s3-website-us-east-1.amazonaws.com/


## TLC-App Structure Explanation ##

![image](https://github.com/nazarenoventrelli/TLC/assets/40725187/81d498af-a1fd-4c6e-93e6-78ac6ce6dedb)


This repo intends to solve the problem of scrape and query tlc data including any date and changing schemas for the different parquet files in the site.

For achieve this i created two main lambdas

**Tlc-Scraper:** Triggered monthly by a CloudWatch event, this lambda extract the links for the current month and upload it to an S3 Bucket that is procesed with a Glue Crawler for extract the schema.

**Tlc-query-handler:**  The query handler uses Athena for query the data processed previously and is exposed using an Api Gateway.


The rest of the components are:

**a Simple UI:** Based on angular, since Athena returns raw jsons is possible just present it using an anguular-material-table. The site is stored in an S3 Bucket and exposed using CloudFront.

**CloudWatch Logs:**  Enabled for logs insights and monitoring.
