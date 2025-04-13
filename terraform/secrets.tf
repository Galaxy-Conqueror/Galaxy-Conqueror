resource "random_password" "password" {
  length = 16
  special = true
  override_special = "!#$&*()-=+[]{}<>:?"
}

resource "aws_secretsmanager_secret" "galaxy-conqueror-secret" {
  name = "galaxy-conqueror-rds-db-secrets"
}

resource "aws_secretsmanager_secret_version" "s-version" {
  secret_id = aws_secretsmanager_secret.galaxy-conqueror-secret.id
  secret_string = <<EOF
    {
      "username": "myuser",
      "password": "${random_password.password.result}"
    }
  EOF

}