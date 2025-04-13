
resource "aws_iam_role" "ecs_task_exec_role" {
  name = "${var.ecs_cluster_name}-task-exec-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_iam_policy" "ecs_task_exec_policy" {
  name = "${var.ecs_cluster_name}-task-exec-policy"
  description = "Policy for ECS task execution role"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "ecr:GetAuthorizationToken",
          "ecr:BatchCheckLayerAvailability",
          "ecr:GetDownloadUrlForLayer",
          "ecr:BatchGetImage",
          "logs:CreateLogStream",
          "logs:PutLogEvents",
          "logs:CreateLogGroup"
        ],
        Effect = "Allow",
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_policy_attachment" "ecs_task_policy_att" {
  name       = "${var.ecs_cluster_name}-task-exec-policy-attachment"
  policy_arn = aws_iam_policy.ecs_task_exec_policy.arn
  roles = [aws_iam_role.ecs_task_exec_role.name]
}

resource "aws_iam_role" "ecs_task_role" {
  name = "${var.ecs_cluster_name}-task-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_iam_policy" "ecs_task_policy" {
  name = "${var.ecs_cluster_name}-ecs-task-policy"
  description = "Policy for ECS task role"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "rds:Connect"
        ],
        Effect = "Allow",
        Resource = "*"
      }
    ]
  })
}

