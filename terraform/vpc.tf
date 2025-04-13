resource "aws_vpc" "main" {

  cidr_block = "10.0.0.0/16"

  enable_dns_support = true
  enable_dns_hostnames = true

  tags = {
    Name = "galaxy-conqueror-vpc"
  }

}

resource "aws_internet_gateway" "gw" {

  vpc_id = aws_vpc.main.id
  tags = {
    Name = "main-int-gateway"
  }

}

resource "aws_subnet" "public-subnet" {
  vpc_id = aws_vpc.main.id
  cidr_block = "10.0.1.0/24"
  availability_zone = "af-south-1a"
  map_public_ip_on_launch = true

  tags = {
    Name = "galaxy-conqueror-public-subnet"
  }
}

resource "aws_subnet" "private-subnet" {
  vpc_id = aws_vpc.main.id

  cidr_block = "10.0.2.0/24"
  availability_zone = "af-south-1a"

  tags = {
    Name = "galaxy-conqueror-private-subnet"
  }

}

resource "aws_subnet" "private-subnet2" {
  vpc_id = aws_vpc.main.id

  cidr_block = "10.0.3.0/24"
  availability_zone = "af-south-1b"

  tags = {
    Name = "galaxy-conqueror-private-subnet"
  }
}

resource "aws_nat_gateway" "nat" {
  allocation_id = aws_eip.nat.id
  subnet_id = aws_subnet.public-subnet.id

  tags = {
    Name = "nat-gateway"
  }
}

resource "aws_eip" "nat" {
  domain = "vpc"
}

resource "aws_route_table" "public-route-table" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.gw.id
  }

  tags = {
    Name = "public-route-table"
  }

}

resource "aws_route_table" "private-route-table" {
  vpc_id = aws_vpc.main.id
  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_nat_gateway.nat.id
  }

  tags = {
    Name = "private-route-table"
  }
}

resource "aws_route_table" "private-route-table2" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_nat_gateway.nat.id
  }

  tags = {
    Name = "private-route-table"
  }
}

resource "aws_route_table_association" "public-assoc" {
  route_table_id = aws_route_table.public-route-table.id
  subnet_id = aws_subnet.public-subnet.id
}
resource "aws_route_table_association" "private-assoc" {
  route_table_id = aws_route_table.private-route-table.id
  subnet_id = aws_subnet.private-subnet.id
}
resource "aws_route_table_association" "private-assoc2" {
  route_table_id = aws_route_table.private-route-table2.id
  subnet_id = aws_subnet.private-subnet2.id
}