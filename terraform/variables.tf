variable "region" {
  type = string
  default = "af-south-1"
}

variable "database_name" {
  type = string
  default = "galaxyconquerordb"
}

variable "instance_name" {
  type = string
  default = "galaxyConquerorDbInstance"
}

variable "ecr_repo_name" {
  type = string
  default = "galaxy-conqueror"
}

variable "ecs_cluster_name" {
  type = string
  default = "galaxy_conqueror_ecs_cluster"
}

variable "ecs_service_name" {
  type = string
  default = "galaxy-conqueror-api-service"
}