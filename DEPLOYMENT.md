# SciConnect - DigitalOcean Deployment Guide

Complete guide to deploying SciConnect on DigitalOcean using Docker Compose.

## Prerequisites

- DigitalOcean account
- A domain name (optional but recommended)
- GitHub account (for code repository)
- Basic command line knowledge

## Estimated Monthly Cost

- **Basic Setup**: $12-18/month (4GB RAM Droplet)
- **Recommended**: $24/month (8GB RAM Droplet) for better performance
- Includes: Compute, bandwidth (1TB included)

---

## Step 1: Create DigitalOcean Droplet

1. **Log in to DigitalOcean** → Create → Droplets

2. **Choose Configuration**:
   - **Image**: Ubuntu 22.04 LTS
   - **Plan**: Basic
   - **CPU Options**: Regular
   - **Size**: 
     - Minimum: 4GB RAM / 2 vCPUs ($18/month)
     - Recommended: 8GB RAM / 4 vCPUs ($48/month)
   - **Datacenter**: Choose closest to your users
   - **VPC Network**: Default
   - **Authentication**: 
     - ✅ SSH Key (recommended) - add your public key
     - Or Password (less secure)

3. **Additional Options**:
   - ✅ Enable: Monitoring (free)
   - ✅ Enable: IPv6 (optional)

4. **Click "Create Droplet"** and wait ~60 seconds

5. **Note your Droplet's IP address** (shown in the dashboard)

---

## Step 2: Configure Domain (Optional but Recommended)

### Option A: Using DigitalOcean DNS
1. Go to **Networking** → **Domains** → **Add Domain**
2. Enter your domain name
3. Add DNS records:
   ```
   A Record:    @              → Your_Droplet_IP
   A Record:    www            → Your_Droplet_IP
   CNAME:       rabbitmq       → @  (optional, for RabbitMQ admin)
   ```

### Option B: Using External DNS Provider
1. Log in to your domain registrar (GoDaddy, Namecheap, etc.)
2. Update DNS A Records:
   ```
   A Record:    @              → Your_Droplet_IP
   A Record:    www            → Your_Droplet_IP
   ```
3. Wait 1-24 hours for DNS propagation

---

## Step 3: Push Code to GitHub

On your **local machine**:

```bash
cd c:\Users\korisnik\Desktop\SciConnect
git add .
git commit -m "Add production deployment files"
git push origin main
```

---

## Step 4: Connect to Your Droplet

### Windows (PowerShell):
```powershell
ssh root@YOUR_DROPLET_IP
```

### Accept the fingerprint prompt (type `yes`)

---

## Step 5: Run Automated Deployment Script

Once connected to your droplet:

```bash
# Download and run the deployment script
curl -o deploy.sh https://raw.githubusercontent.com/YOUR_USERNAME/SciConnect/main/SciConnect/deploy.sh
chmod +x deploy.sh
sudo ./deploy.sh
```

The script will:
- ✅ Install Docker and Docker Compose
- ✅ Configure firewall
- ✅ Clone your repository
- ✅ Set up environment variables
- ✅ Build Docker images
- ✅ Start all services

### During script execution:
1. **Enter your GitHub repository URL** when prompted:
   ```
   https://github.com/RS2-Crew/SciConnect.git
   ```

2. **Edit the `.env` file** when the editor opens:
   - Replace `YourStrongPassword123!` with actual strong passwords
   - Update email settings
   - Update `DOMAIN=yourdomain.com` with your actual domain
   - Save and exit (Ctrl+X, then Y, then Enter)

---

## Step 6: Set Up SSL Certificates (HTTPS)

### Option A: Let's Encrypt (Free, Recommended)

```bash
cd /opt/sciconnect/SciConnect

# Install Certbot
sudo apt-get install certbot -y

# Stop nginx temporarily
docker compose -f docker-compose.prod.yml stop nginx

# Get certificate (replace yourdomain.com)
sudo certbot certonly --standalone -d yourdomain.com -d www.yourdomain.com

# Copy certificates to nginx directory
sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem nginx/ssl/
sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem nginx/ssl/

# Start nginx
docker compose -f docker-compose.prod.yml start nginx
```

### Option B: Self-Signed Certificate (Testing Only)

```bash
cd /opt/sciconnect/SciConnect/nginx/ssl

# Generate self-signed certificate
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout privkey.pem -out fullchain.pem

# Restart nginx
docker compose -f docker-compose.prod.yml restart nginx
```

### Auto-Renew Let's Encrypt Certificates

```bash
# Test renewal
sudo certbot renew --dry-run

# Add cron job for auto-renewal
echo "0 3 * * * root certbot renew --quiet && docker compose -f /opt/sciconnect/SciConnect/docker-compose.prod.yml restart nginx" | sudo tee -a /etc/crontab
```

---

## Step 7: Update Nginx Configuration with Your Domain

```bash
cd /opt/sciconnect/SciConnect

# Edit nginx configuration
nano nginx/conf.d/sciconnect.conf
```

**Replace all instances of `yourdomain.com` with your actual domain**

Press `Ctrl+X`, then `Y`, then `Enter` to save.

```bash
# Restart nginx
docker compose -f docker-compose.prod.yml restart nginx
```

---

## Step 8: Verify Deployment

### Check if all services are running:
```bash
cd /opt/sciconnect/SciConnect
docker compose -f docker-compose.prod.yml ps
```

All services should show "running" status.

### Check logs:
```bash
# View all logs
docker compose -f docker-compose.prod.yml logs

# View specific service
docker compose -f docker-compose.prod.yml logs identityservice

# Follow logs in real-time
docker compose -f docker-compose.prod.yml logs -f
```

### Test the application:
- **Frontend**: https://yourdomain.com
- **API Health**: https://yourdomain.com/api/health (if configured)
- **RabbitMQ**: https://rabbitmq.yourdomain.com

---

## Step 9: Initialize Database

The database migrations should run automatically on first startup. If not:

```bash
cd /opt/sciconnect/SciConnect

# Access the main service container
docker exec -it mainservice bash

# Run migrations (if needed)
dotnet ef database update

# Exit container
exit
```

---

## Common Commands

### View running containers:
```bash
docker compose -f docker-compose.prod.yml ps
```

### View logs:
```bash
docker compose -f docker-compose.prod.yml logs -f [service_name]
```

### Restart a service:
```bash
docker compose -f docker-compose.prod.yml restart [service_name]
```

### Stop all services:
```bash
docker compose -f docker-compose.prod.yml down
```

### Start all services:
```bash
docker compose -f docker-compose.prod.yml up -d
```

### Rebuild after code changes:
```bash
git pull
docker compose -f docker-compose.prod.yml up -d --build
```

### View resource usage:
```bash
docker stats
```

---

## Troubleshooting

### Services won't start:
```bash
# Check logs
docker compose -f docker-compose.prod.yml logs

# Check if ports are in use
sudo netstat -tulpn | grep LISTEN
```

### Database connection issues:
```bash
# Ensure SQL Server is healthy
docker compose -f docker-compose.prod.yml ps mssql

# Check SQL Server logs
docker compose -f docker-compose.prod.yml logs mssql
```

### Cannot access website:
1. Check firewall: `sudo ufw status`
2. Verify nginx is running: `docker ps | grep nginx`
3. Check nginx logs: `docker compose -f docker-compose.prod.yml logs nginx`
4. Verify DNS: `nslookup yourdomain.com`

### SSL certificate issues:
```bash
# Check certificate files exist
ls -la nginx/ssl/

# Test nginx configuration
docker compose -f docker-compose.prod.yml exec nginx nginx -t

# Reload nginx
docker compose -f docker-compose.prod.yml exec nginx nginx -s reload
```

---

## Security Best Practices

### 1. Change Default Passwords
Edit `.env` file and update all passwords:
```bash
nano /opt/sciconnect/SciConnect/.env
```

### 2. Set Up Firewall Rules
```bash
# Only allow necessary ports
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw enable
```

### 3. Set Up SSH Key Authentication
If using password, switch to SSH keys:
```bash
# On your local machine, generate SSH key
ssh-keygen -t rsa -b 4096

# Copy to droplet
ssh-copy-id root@YOUR_DROPLET_IP

# Disable password authentication
sudo nano /etc/ssh/sshd_config
# Set: PasswordAuthentication no
sudo systemctl restart sshd
```

### 4. Enable Automatic Security Updates
```bash
sudo apt-get install unattended-upgrades -y
sudo dpkg-reconfigure -plow unattended-upgrades
```

### 5. Set Up Monitoring
- Enable DigitalOcean monitoring in droplet settings
- Set up alerts for CPU/Memory/Disk usage

---

## Backup Strategy

### Database Backup Script:
```bash
# Create backup directory
mkdir -p /opt/backups

# Create backup script
cat > /opt/backups/backup.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/opt/backups"
DATE=$(date +%Y%m%d_%H%M%S)
cd /opt/sciconnect/SciConnect

# Backup databases
docker exec mssql /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "$DB_SA_PASSWORD" \
  -Q "BACKUP DATABASE IdentityDb TO DISK = '/var/opt/mssql/backup/IdentityDb_${DATE}.bak'"

docker exec mssql /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "$DB_SA_PASSWORD" \
  -Q "BACKUP DATABASE EntityDb TO DISK = '/var/opt/mssql/backup/EntityDb_${DATE}.bak'"

# Copy backups
docker cp mssql:/var/opt/mssql/backup/ ${BACKUP_DIR}/

# Delete backups older than 7 days
find ${BACKUP_DIR} -name "*.bak" -mtime +7 -delete
EOF

chmod +x /opt/backups/backup.sh

# Schedule daily backups (2 AM)
echo "0 2 * * * root /opt/backups/backup.sh" | sudo tee -a /etc/crontab
```

---

## Scaling Considerations

### Increase Droplet Resources:
1. Go to DigitalOcean Dashboard
2. Select your droplet
3. Click "Resize"
4. Choose larger plan
5. Services will restart automatically

### Monitor Performance:
```bash
# Check resource usage
docker stats

# Check disk space
df -h

# Check memory
free -h
```

---

## Updating the Application

```bash
cd /opt/sciconnect/SciConnect

# Pull latest code
git pull origin main

# Rebuild and restart
docker compose -f docker-compose.prod.yml up -d --build

# View logs to ensure successful startup
docker compose -f docker-compose.prod.yml logs -f
```

---

## Cost Optimization

1. **Start with 4GB droplet** ($18/month), upgrade if needed
2. **Use DigitalOcean Spaces** for file storage if needed
3. **Enable monitoring** to track actual resource usage
4. **Set up alerts** to avoid overages

---

## Support

- **DigitalOcean Docs**: https://docs.digitalocean.com/
- **Docker Docs**: https://docs.docker.com/
- **Let's Encrypt**: https://letsencrypt.org/docs/

---

## Quick Reference

| What | Where |
|------|-------|
| Application Directory | `/opt/sciconnect/SciConnect` |
| Environment Variables | `/opt/sciconnect/SciConnect/.env` |
| Nginx Config | `/opt/sciconnect/SciConnect/nginx/conf.d/sciconnect.conf` |
| SSL Certificates | `/opt/sciconnect/SciConnect/nginx/ssl/` |
| Docker Compose File | `/opt/sciconnect/SciConnect/docker-compose.prod.yml` |
| Logs | `docker compose -f docker-compose.prod.yml logs` |

---

**Your application should now be live at https://yourdomain.com! 🚀**
