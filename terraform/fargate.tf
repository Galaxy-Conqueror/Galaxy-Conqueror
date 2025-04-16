
data "aws_caller_identity" "current" {}

resource "aws_ecs_task_definition" "ecs_task_definition" {
  family                = "fargate-task"
  execution_role_arn = aws_iam_role.ecs_task_exec_role.arn
  network_mode = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu = 256
  memory = 512

  task_role_arn = aws_iam_role.ecs_task_role.arn
  container_definitions = jsonencode([
    {
      name = "galaxy-conqueror-api"
      image = "${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.region}.amazonaws.com/${var.ecr_repo_name}:latest"
      portMappings = [
        {
          containerPort = 8080
          hostPort      = 8080
          protocol = "tcp"
        }
      ]
      environment: [
        {
          "name": "DB_HOST",
          "value": "${aws_db_instance.main.endpoint}"
        },
        {
          "name": "DB_NAME",
          "value": "${aws_db_instance.main.db_name}"
        },
        {
          "name": "DB_USERNAME",
          "value": local.db_creds.username
        },
        {
          "name": "DB_PASSWORD",
          "value": local.db_creds.password
        },
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        }

      ],
      secrets = [
        {
          name      = "GOOGLE_CLIENT_ID"
          valueFrom = aws_secretsmanager_secret.google_client_id.arn
        },
        {
          name      = "GOOGLE_CLIENT_SECRET"
          valueFrom = aws_secretsmanager_secret.google_client_secret.arn
        },
        {
          name      = "ANTHROPIC_API_KEY"
          valueFrom = aws_secretsmanager_secret.anthropic_api_key.arn
        }
      ],
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group = aws_cloudwatch_log_group.ecs_logs.name
          awslogs-create-group = "true"
          awslogs-region = var.region
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])
}

resource "aws_ecs_service" "ecs_service" {
  name = var.ecs_service_name
  cluster = aws_ecs_cluster.ecs_cluster.id
  task_definition = aws_ecs_task_definition.ecs_task_definition.arn
  desired_count = 1
  launch_type = "FARGATE"

  network_configuration {
    subnets = [aws_subnet.public-subnet.id]
    security_groups = [aws_security_group.fargate_sg.id]
    assign_public_ip = true
  }

  force_new_deployment = true

}

resource "aws_ecs_task_definition" "flyway_task" {
  family                   = "flyway-task"
  execution_role_arn       = aws_iam_role.ecs_task_exec_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 256
  memory                   = 512

  container_definitions = jsonencode([
    {
      name  = "flyway"
      image = "${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.region}.amazonaws.com/${var.ecr_repo_name}:flyway"
      essential = true
      environment = [
        { name = "FLYWAY_URL",     value = "jdbc:postgresql://${aws_db_instance.main.endpoint}/${aws_db_instance.main.db_name}" },
        { name = "FLYWAY_USER",    value = local.db_creds.username },
        { name = "FLYWAY_PASSWORD", value = local.db_creds.password }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = aws_cloudwatch_log_group.ecs_logs.name
          awslogs-create-group  = "true"
          awslogs-region        = var.region
          awslogs-stream-prefix = "flyway"
        }
      }
    }
  ])
}
