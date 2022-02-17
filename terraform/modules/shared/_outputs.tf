output "shared_table" {
  value = aws_dynamodb_table.shared_dynamodb_table
  description = "The name of the shared dynamodb table."
}
