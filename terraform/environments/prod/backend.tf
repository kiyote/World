
# For deploying AWS resources
provider "aws" {
  region = "ca-central-1"

  assume_role {
    role_arn = "arn:aws:iam::860568434255:role/world_deploy_prod"
  }
}

# For managing the terraform state
terraform {
  backend "s3" {
    bucket = "kiyote.terraformstate"
    use_lockfile = true
    key    = "prod/world"
    region = "ca-central-1"
    assume_role = {
      role_arn = "arn:aws:iam::860568434255:role/terraform_state_prod"
    }
  }
}
