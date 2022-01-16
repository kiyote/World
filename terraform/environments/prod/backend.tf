
provider "aws" {
  region = "ca-central-1"

  assume_role = {
    role_arn = "arn:aws:iam::860568434255:role/terraform_world_prod"
  }
}

terraform {
  backend "s3" {
    bucket = "terraformstate6709d8b42a0c40a6be48b840ec9b12ff"
    key    = "prod/world"
    region = "ca-central-1"
  }
}