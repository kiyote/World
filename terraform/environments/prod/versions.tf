terraform {
  required_version = ">= 1.13.3"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "6.15.0"
    }
    null = {
      source = "hashicorp/null"
      version = "3.2.4"
    }    
  }
}