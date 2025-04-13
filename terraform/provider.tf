terraform {

  backend "s3" {
    bucket = "galaxy-conqueror-tf-state"
    dynamodb_table = "galaxy-conqueror-state-lock-table"
    encrypt = true
    region = "af-south-1"
    key = "galaxy-conqueror/main.tfstate"
  }

  required_providers {
    aws = {
      source = "hashicorp/aws"
      version = "5.86.1"
    }

  }
}

provider "aws" {
  region = "af-south-1"
}