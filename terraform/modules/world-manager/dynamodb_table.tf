
resource "aws_dynamodb_table" "world_manager_table" {
  name = "${var.object_prefix}_world_manager"
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
}