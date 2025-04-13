provider "aws" {
  region = "af-south-1"
}

resource "aws_s3_bucket" "tf_state" {
  bucket = "galaxy-conqueror-tf-state"
}

resource "aws_dynamodb_table" "tf_lock" {
  name         = "galaxy-conqueror-state-lock-table"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "LockID"

  attribute {
    name = "LockID"
    type = "S"
  }
}