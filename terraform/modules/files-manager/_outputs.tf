output "files_bucket_name" {
  value = aws_s3_bucket.files_bucket.name
  description = "The name of the files bucket."
}
