variable "aws_region" {
  default = "us-east-1"
}

variable "ami_id" {
  description = "AMI for EC2 instance"
  default     = "ami-0c02fb55956c7d316" # Ubuntu 22.04 in us-east-1
}

variable "instance_type" {
  default = "t2.micro"
}

variable "key_pair" {
  description = "Name of the AWS key pair"
}

variable "allowed_ip" {
  description = "IP allowed for SSH access (e.g. your public IP/32)"
}
