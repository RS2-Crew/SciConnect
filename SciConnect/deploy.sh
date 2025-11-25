#!/bin/bash

# SciConnect Deployment Script for DigitalOcean
# This script sets up Docker, pulls the repository, and deploys the application

set -e

echo "=================================="
echo "SciConnect Deployment Script"
echo "=================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}→ $1${NC}"
}

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    print_error "Please run as root (use sudo)"
    exit 1
fi

print_info "Step 1: Updating system packages..."
apt-get update -y
apt-get upgrade -y
print_success "System updated"

print_info "Step 2: Installing required packages..."
apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
    git \
    ufw
print_success "Required packages installed"

print_info "Step 3: Installing Docker..."
if ! command -v docker &> /dev/null; then
    # Add Docker's official GPG key
    install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    chmod a+r /etc/apt/keyrings/docker.gpg

    # Add Docker repository
    echo \
      "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
      $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null

    # Install Docker
    apt-get update -y
    apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
    
    # Start and enable Docker
    systemctl start docker
    systemctl enable docker
    
    print_success "Docker installed successfully"
else
    print_success "Docker is already installed"
fi

print_info "Step 4: Configuring firewall..."
# Configure UFW
ufw --force enable
ufw default deny incoming
ufw default allow outgoing
ufw allow 22/tcp    # SSH
ufw allow 80/tcp    # HTTP
ufw allow 443/tcp   # HTTPS
ufw reload
print_success "Firewall configured"

print_info "Step 5: Creating application directory..."
APP_DIR="/opt/sciconnect"
mkdir -p $APP_DIR
cd $APP_DIR
print_success "Application directory created: $APP_DIR"

print_info "Step 6: Cloning repository..."
read -p "Enter your Git repository URL (or press Enter to skip): " REPO_URL
if [ -n "$REPO_URL" ]; then
    if [ -d ".git" ]; then
        git pull
        print_success "Repository updated"
    else
        git clone $REPO_URL .
        print_success "Repository cloned"
    fi
else
    print_info "Skipping repository clone. Please manually copy your files to $APP_DIR"
fi

print_info "Step 7: Setting up environment variables..."
if [ ! -f ".env" ]; then
    if [ -f "SciConnect/.env.example" ]; then
        cp SciConnect/.env.example SciConnect/.env
        print_info "Please edit SciConnect/.env with your actual credentials"
        print_info "Opening editor in 5 seconds..."
        sleep 5
        nano SciConnect/.env
        print_success "Environment file created"
    else
        print_error ".env.example not found"
    fi
else
    print_success "Environment file already exists"
fi

print_info "Step 8: Setting up SSL certificates..."
mkdir -p SciConnect/nginx/ssl
print_info "SSL certificates directory created"
print_info "You can use Let's Encrypt (see deployment guide) or place your certificates here"

print_info "Step 9: Building and starting containers..."
cd SciConnect
docker compose -f docker-compose.prod.yml build
print_success "Docker images built"

print_info "Step 10: Starting services..."
docker compose -f docker-compose.prod.yml up -d
print_success "Services started"

echo ""
echo "=================================="
print_success "Deployment completed!"
echo "=================================="
echo ""
echo "Next steps:"
echo "1. Point your domain DNS A record to this server's IP"
echo "2. Set up SSL certificates (see DEPLOYMENT.md)"
echo "3. Update nginx configuration with your domain"
echo "4. Restart nginx: docker compose -f docker-compose.prod.yml restart nginx"
echo ""
echo "Useful commands:"
echo "  View logs: docker compose -f docker-compose.prod.yml logs -f"
echo "  Stop: docker compose -f docker-compose.prod.yml down"
echo "  Restart: docker compose -f docker-compose.prod.yml restart"
echo ""
