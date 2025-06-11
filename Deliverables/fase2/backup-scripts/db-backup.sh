#!/usr/bin/bash

# This script is used to backup the database
# It is run by the cron job every day at 2:00 AM
# It creates a backup of the database and stores it in the /var/backups directory
# Backups are stored in a compressed format and encrypted with a password
# backups performed on sundays are kept for 30 days, while backups performed on other days are kept for 7 days
# The first backup of a month is kept for a year
# The first backup of the year is kept for 5 years

# Variables
BACKUP_DIR="/var/backups"
DB_NAME="your_database_name"
DB_USER="your_database_user"
DB_PASSWORD="your_database_password"
DB_SERVER="server"
ENCRYPTION_PASSWORD="your_encryption_password"
DATE=$(date +%Y-%m-%d)

# Create backup directory if it doesn't exist
mkdir -p "$BACKUP_DIR/{daily, weekly,monthly,yearly}"

# Create a backup of the database using mysqldump
BACKUP_FILE="$BACKUP_DIR/db_backup_$DATE.sql.gz"
mysqldump -u "$DB_USER" -p"$DB_PASSWORD" -h "$DB_SERVER" "$DB_NAME" | gzip > "$BACKUP_FILE"

# Encrypt the backup file
openssl enc -aes-256-cbc -salt -in "$BACKUP_FILE" -out "$BACKUP_FILE.enc" -k "$ENCRYPTION_PASSWORD"

# Remove the unencrypted backup file
rm "$BACKUP_FILE"

# Determine retention policy based on the day of the week
# If it is sunday
if [ "$(date +%u)" -eq 7 ]; then
    # Copy backup to weekly directory
    find "$BACKUP_DIR/weekly" -type f -name "*.enc" -mtime +30 -exec rm {} \;
    cp "$BACKUP_FILE.enc" "$BACKUP_DIR/weekly/db_backup_$DATE.enc"
    # If it is the first sunday of the month
    if [[ "$(date +%d)" -le 7 ]]; then
        # Keep backups for 1 year
        find "$BACKUP_DIR/monthly" -type f -name "*.enc" -mtime +365 -exec rm {} \;
        cp "$BACKUP_FILE.enc" "$BACKUP_DIR/monthly/db_backup_$DATE.enc"
    fi
    # If it is the first sunday of the year
    if [[ "$(date +%j)" -le 7 ]]; then
        # Keep backups for 5 years
        find "$BACKUP_DIR/yearly" -type f -name "*.enc" -mtime +1825 -exec rm {} \;
        cp "$BACKUP_FILE.enc" "$BACKUP_DIR/yearly/db_backup_$DATE.enc"
    fi
else
    # Copy backup to daily directory
    find "$BACKUP_DIR/daily" -type f -name "*.enc" -mtime +7 -exec rm {} \;
    cp "$BACKUP_FILE.enc" "$BACKUP_DIR/daily/db_backup_$DATE.enc"
fi

## Delete temporary file
rm -f "$BACKUP_FILE.enc"