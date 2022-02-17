output "shared_table_name" {
  value = aws_dynamodb_table.shared_dynamodb_table.name
  description = "The name of the shared dynamodb table."
}
