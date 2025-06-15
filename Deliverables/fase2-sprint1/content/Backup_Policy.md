## Backup Policy

### Backup Policy Overview

#### Purpose
The purpose of this backup policy is to ensure the integrity, availability, and recoverability of critical data within the ShopTex application. Regular backups are essential to protect against data loss due to accidental deletion, corruption, or system failures.

#### Scope
This policy applies to all data stored within the ShopTex application (databases).

### Backup Schedule

#### Frequency 
Backups will be performed daily, at a time outside regular use (2am). 

#### Retention Period 
Backups are retained for a period of 7 days, with the exceptions of:
   - **Sunday backups**: Retained for a maximum of 30 days.
   - **First sunday of the month**: Retained for a maximum of a year.
   - **First sunday of the year**: Retained for a maximum of 5 years.

Backups older than the retention period will be automatically deleted.

### Backup Types
#### Full Backups
Full backups will be performed daily, capturing the entire state of the database and application data. This ensures that a complete snapshot is available for restoration.

#### Incremental Backups
No incremental backups will be performed. Each backup will be a full backup to simplify the restoration process.

### Backup Storage
#### Location
Backups will be store in a separate server (vs235), owned by a different student than the server hosting the application. This ensures that backups are not affected by issues on the primary server.

#### Security
Backups will be encrypted, after compression, to protect sensitive data using AES file encryption. Access to files will be restricted to authorized personnel only.

### Restoration Procedures
#### Testing
Restoration procedures will be tested quarterly to ensure that backups can be successfully restored. This includes verifying the integrity of the backup files and the restoration process.

#### Restoration Steps
1. Identify the backup file to restore based on the date and time of the desired state.
2. Decrypt the backup file using the appropriate encryption key.
3. Restore the database and application data to the target environment using native mysql tools.
4. Verify the integrity of the restored data by running integrity checks and validating application functionality.
5. Document the restoration process and any issues encountered for future reference.
