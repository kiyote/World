
resource "aws_dynamodb_table" "shared_dynamodb_table" {
  name = "${var.object_prefix}table"
  billing_mode = "PAY_PER_REQUEST"
  hash_key = "PK"
  range_key = "SK"

  attribute {
	name = "PK"  
	type = "S"
  }

  attribute {
     name = "SK"
	 type = "S"
  }

  global_secondary_index {
    name            = "GSI"
    hash_key        = "SK"
    range_key       = "PK"
    projection_type = "ALL"
  }

  ttl {
    attribute_name = "timeout"
    enabled        = true
  }
}