locals {
  db_creds = jsondecode(aws_secretsmanager_secret_version.s-version.secret_string)
}

resource "aws_db_subnet_group" "default" {
  name = "main-rds-subnet-group"
  subnet_ids = [aws_subnet.private-subnet.id, aws_subnet.private-subnet2.id]

  tags = {
    Name = "rds-subnet-grp"
  }

}

resource "aws_db_instance" "main" {
  allocated_storage = 20
  identifier = var.database_name
  db_name = var.database_name
  instance_class = "db.t3.micro"
  engine = "postgres"
  engine_version = "17.2"
  username = local.db_creds.username
  password = local.db_creds.password
  parameter_group_name = "default.postgres17"
  skip_final_snapshot = true
  publicly_accessible = false
  db_subnet_group_name = aws_db_subnet_group.default.name
  vpc_security_group_ids = [aws_security_group.rds-sec-grp.id]

  tags = {
    Name = "galaxy-conqueror-rds"
  }
}