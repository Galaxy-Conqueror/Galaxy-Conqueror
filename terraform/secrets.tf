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

resource "aws_secretsmanager_secret" "google_client_id" {
  name = "google-client-id"
}

resource "aws_secretsmanager_secret_version" "google_client_id_version" {
  secret_id     = aws_secretsmanager_secret.google_client_id.id
  secret_string = var.google_client_id
}

resource "aws_secretsmanager_secret" "google_client_secret" {
  name = "google-client-secret"
}

resource "aws_secretsmanager_secret_version" "google_client_secret_version" {
  secret_id     = aws_secretsmanager_secret.google_client_secret.id
  secret_string = var.google_client_secret
}

resource "aws_secretsmanager_secret" "anthropic_api_key" {
  name = "anthropic-api-key"
}

resource "aws_secretsmanager_secret_version" "anthropic_api_key_version" {
  secret_id     = aws_secretsmanager_secret.anthropic_api_key.id
  secret_string = var.anthropic_api_key
}
