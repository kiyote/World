
provider "aws" {
  region = "ca-central-1"
}

terraform {
  backend "s3" {
    bucket = "terraformstate6709d8b42a0c40a6be48b840ec9b12ff"
    dynamodb_table = "terraformstate6709d8b42a0c40a6be48b840ec9b12ff"
    key    = "prod/world"
    region = "ca-central-1"
    role_arn = "arn:aws:iam::860568434255:role/terraform_world_dev"
  }
}