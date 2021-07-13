/*
 Navicat MySQL Data Transfer

 Source Server         : comet
 Source Server Type    : MariaDB
 Source Server Version : 100410
 Source Host           : localhost:3306
 Source Schema         : account_zf

 Target Server Type    : MariaDB
 Target Server Version : 100410
 File Encoding         : 65001

 Date: 13/07/2021 15:56:04
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account`  (
  `AccountID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Username` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Password` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Salt` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `AuthorityID` smallint(6) UNSIGNED NOT NULL DEFAULT 1,
  `StatusID` smallint(6) UNSIGNED NOT NULL DEFAULT 1,
  `IPAddress` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `MacAddress` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Registered` datetime NOT NULL DEFAULT current_timestamp(),
  `VipLevel` tinyint(3) UNSIGNED NOT NULL DEFAULT 0,
  `ParentIdentity` int(4) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`AccountID`) USING BTREE,
  UNIQUE INDEX `AccountID_UNIQUE`(`AccountID`) USING BTREE,
  UNIQUE INDEX `Username_UNIQUE`(`Username`) USING BTREE,
  INDEX `fk_account_account_authority_idx`(`AuthorityID`) USING BTREE,
  INDEX `fk_account_account_status_idx`(`StatusID`) USING BTREE,
  CONSTRAINT `fk_account_account_authority` FOREIGN KEY (`AuthorityID`) REFERENCES `account_authority` (`AuthorityID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_account_account_status` FOREIGN KEY (`StatusID`) REFERENCES `account_status` (`StatusID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of account
-- ----------------------------
INSERT INTO `account` VALUES (1, 'inburst', 'a380ce770774ddae145305b1f4896b5812eafb6d2f609c79fa70814866cd33c0', '8fc74330ab7609841f832d461d1f6981', 5, 1, NULL, '', '2021-07-12 23:25:47', 0, 0);
INSERT INTO `account` VALUES (2, 'inburst2', '9c5f852d2ade7e63411efc0a11b98995e8f3846730dffcc716fc835c34b77a07', 'c3ca9bcabe095f3ba1c06f2e97bb10db', 5, 1, NULL, '', '2021-07-13 15:33:40', 0, 0);

-- ----------------------------
-- Table structure for account_authority
-- ----------------------------
DROP TABLE IF EXISTS `account_authority`;
CREATE TABLE `account_authority`  (
  `AuthorityID` smallint(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `AuthorityName` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`AuthorityID`) USING BTREE,
  UNIQUE INDEX `AuthorityID_UNIQUE`(`AuthorityID`) USING BTREE,
  UNIQUE INDEX `AuthorityName_UNIQUE`(`AuthorityName`) USING BTREE,
  INDEX `AuthorityID`(`AuthorityID`) USING BTREE,
  INDEX `AuthorityID_2`(`AuthorityID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of account_authority
-- ----------------------------
INSERT INTO `account_authority` VALUES (5, 'Administrator');
INSERT INTO `account_authority` VALUES (3, 'Game Manager');
INSERT INTO `account_authority` VALUES (2, 'Moderator');
INSERT INTO `account_authority` VALUES (1, 'Player');
INSERT INTO `account_authority` VALUES (4, 'Project Manager');

-- ----------------------------
-- Table structure for account_ban
-- ----------------------------
DROP TABLE IF EXISTS `account_ban`;
CREATE TABLE `account_ban`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Type` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `AccountId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `BanTime` datetime NOT NULL DEFAULT current_timestamp(),
  `ExpireTime` datetime NOT NULL DEFAULT '2199-12-31 23:59:59',
  `BannedBy` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Reason` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT '',
  `UpdatedAt` datetime NULL DEFAULT NULL,
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of account_ban
-- ----------------------------

-- ----------------------------
-- Table structure for account_status
-- ----------------------------
DROP TABLE IF EXISTS `account_status`;
CREATE TABLE `account_status`  (
  `StatusID` smallint(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `StatusName` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`StatusID`) USING BTREE,
  UNIQUE INDEX `StatusID_UNIQUE`(`StatusID`) USING BTREE,
  UNIQUE INDEX `StatusName_UNIQUE`(`StatusName`) USING BTREE,
  INDEX `StatusID`(`StatusID`) USING BTREE,
  INDEX `StatusID_2`(`StatusID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of account_status
-- ----------------------------
INSERT INTO `account_status` VALUES (2, 'Activated');
INSERT INTO `account_status` VALUES (5, 'Banned');
INSERT INTO `account_status` VALUES (3, 'Limited');
INSERT INTO `account_status` VALUES (4, 'Locked');
INSERT INTO `account_status` VALUES (1, 'Registered');

-- ----------------------------
-- Table structure for articles
-- ----------------------------
DROP TABLE IF EXISTS `articles`;
CREATE TABLE `articles`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `type` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `user_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `section_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `create_date` datetime NOT NULL DEFAULT current_timestamp(),
  `edit_date` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `flag` bigint(16) UNSIGNED NOT NULL DEFAULT 0,
  `del_date` datetime NULL DEFAULT NULL,
  `thumb` int(10) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 17 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of articles
-- ----------------------------

-- ----------------------------
-- Table structure for articles_category
-- ----------------------------
DROP TABLE IF EXISTS `articles_category`;
CREATE TABLE `articles_category`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `order` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `type` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of articles_category
-- ----------------------------

-- ----------------------------
-- Table structure for articles_content
-- ----------------------------
DROP TABLE IF EXISTS `articles_content`;
CREATE TABLE `articles_content`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `article_id` int(4) UNSIGNED NOT NULL,
  `creator_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `locale` varchar(8) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `title` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `last_editor_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `creation_date` datetime NOT NULL DEFAULT current_timestamp(),
  `edit_date` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `article_set`(`article_id`) USING BTREE,
  INDEX `article_owner`(`creator_id`) USING BTREE,
  INDEX `article_editor`(`last_editor_id`) USING BTREE,
  CONSTRAINT `article_editor` FOREIGN KEY (`last_editor_id`) REFERENCES `ftw_account` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `article_owner` FOREIGN KEY (`creator_id`) REFERENCES `ftw_account` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `article_set` FOREIGN KEY (`article_id`) REFERENCES `articles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 17 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of articles_content
-- ----------------------------

-- ----------------------------
-- Table structure for articles_read_control
-- ----------------------------
DROP TABLE IF EXISTS `articles_read_control`;
CREATE TABLE `articles_read_control`  (
  `Identity` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `UserIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `SessionIdentity` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `UserAgent` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `IpAddress` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0.0.0.0',
  `Referer` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of articles_read_control
-- ----------------------------

-- ----------------------------
-- Table structure for credit_card
-- ----------------------------
DROP TABLE IF EXISTS `credit_card`;
CREATE TABLE `credit_card`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Type` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `OwnerId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `CheckoutItemId` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `Part1` smallint(4) UNSIGNED ZEROFILL NOT NULL DEFAULT 0000,
  `Part2` smallint(4) UNSIGNED ZEROFILL NOT NULL DEFAULT 0000,
  `Part3` smallint(4) UNSIGNED ZEROFILL NOT NULL DEFAULT 0000,
  `Part4` smallint(4) UNSIGNED ZEROFILL NOT NULL DEFAULT 0000,
  `Password` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `CreatedAt` datetime NOT NULL,
  `UsedAt` datetime NULL DEFAULT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 14 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of credit_card
-- ----------------------------

-- ----------------------------
-- Table structure for credit_card_usage
-- ----------------------------
DROP TABLE IF EXISTS `credit_card_usage`;
CREATE TABLE `credit_card_usage`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `CardId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `TargetId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `UsedAt` datetime NOT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `cardid`(`CardId`) USING BTREE,
  INDEX `userid`(`TargetId`) USING BTREE,
  CONSTRAINT `ChkCard` FOREIGN KEY (`CardId`) REFERENCES `credit_card` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of credit_card_usage
-- ----------------------------

-- ----------------------------
-- Table structure for credit_card_vip
-- ----------------------------
DROP TABLE IF EXISTS `credit_card_vip`;
CREATE TABLE `credit_card_vip`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `AccountId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `CardId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `CreationDate` datetime NOT NULL DEFAULT current_timestamp(),
  `BoundServerId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `BoundTargetId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `BindDate` datetime NULL DEFAULT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of credit_card_vip
-- ----------------------------

-- ----------------------------
-- Table structure for discord_channel
-- ----------------------------
DROP TABLE IF EXISTS `discord_channel`;
CREATE TABLE `discord_channel`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ChannelId` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `Name` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `CreatedAt` int(11) NOT NULL DEFAULT 0,
  `Default` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of discord_channel
-- ----------------------------

-- ----------------------------
-- Table structure for discord_message
-- ----------------------------
DROP TABLE IF EXISTS `discord_message`;
CREATE TABLE `discord_message`  (
  `Id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `CurrentUserName` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `ChannelId` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `Message` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Timestamp` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 12 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of discord_message
-- ----------------------------

-- ----------------------------
-- Table structure for discord_user
-- ----------------------------
DROP TABLE IF EXISTS `discord_user`;
CREATE TABLE `discord_user`  (
  `Identity` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `DiscordUserId` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `AccountId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `GameUserId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `AccountName` varchar(64) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `GameName` varchar(16) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Name` varchar(64) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Discriminator` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `CreatedAt` int(11) NOT NULL DEFAULT 0,
  `MessagesSent` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `CharactersSent` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of discord_user
-- ----------------------------

-- ----------------------------
-- Table structure for download
-- ----------------------------
DROP TABLE IF EXISTS `download`;
CREATE TABLE `download`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `type` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of download
-- ----------------------------

-- ----------------------------
-- Table structure for download_url
-- ----------------------------
DROP TABLE IF EXISTS `download_url`;
CREATE TABLE `download_url`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `download_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `provider_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `url` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `download_id`(`download_id`) USING BTREE,
  CONSTRAINT `downloadidx` FOREIGN KEY (`download_id`) REFERENCES `download` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of download_url
-- ----------------------------

-- ----------------------------
-- Table structure for ftw_account
-- ----------------------------
DROP TABLE IF EXISTS `ftw_account`;
CREATE TABLE `ftw_account`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `email` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `password` varchar(130) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `type` tinyint(1) UNSIGNED NOT NULL DEFAULT 1,
  `flag` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `salt` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'DEFAULT_PTZF_SALT',
  `hash` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `vip` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `vip_coins` bigint(16) UNSIGNED NOT NULL DEFAULT 0,
  `vip_points` int(4) UNSIGNED NOT NULL DEFAULT 300,
  `security_code` bigint(16) UNSIGNED NOT NULL DEFAULT 0,
  `security_question` int(4) NOT NULL DEFAULT 0,
  `security_answer` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `country` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `language` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `real_name` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `sex` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `age` date NULL DEFAULT NULL,
  `phone` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `netbar_ip` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `creation_date` datetime NULL DEFAULT NULL,
  `first_login` datetime NULL DEFAULT NULL,
  `last_login` datetime NULL DEFAULT NULL,
  `email_confirmed` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `phone_confirmed` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `twofactor_enabled` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `lockout_enabled` tinyint(1) UNSIGNED NOT NULL DEFAULT 1,
  `access_failed_count` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `lockout_end` timestamp NULL DEFAULT NULL,
  `last_activity` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `default_billing_info` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `updated_at` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `email`(`id`) USING BTREE,
  UNIQUE INDEX `idx`(`email`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ftw_account
-- ----------------------------

-- ----------------------------
-- Table structure for ftw_account_vip
-- ----------------------------
DROP TABLE IF EXISTS `ftw_account_vip`;
CREATE TABLE `ftw_account_vip`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `AccountId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `VipCredits` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `VipPoints` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `AccountId`(`AccountId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ftw_account_vip
-- ----------------------------

-- ----------------------------
-- Table structure for ftw_billing_information
-- ----------------------------
DROP TABLE IF EXISTS `ftw_billing_information`;
CREATE TABLE `ftw_billing_information`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `document` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `first_name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `last_name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `address` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `complement` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `district` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `city` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `country` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `post_code` varchar(15) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `email` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `phone` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `additional_info` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ftw_billing_information
-- ----------------------------

-- ----------------------------
-- Table structure for ftw_security_question
-- ----------------------------
DROP TABLE IF EXISTS `ftw_security_question`;
CREATE TABLE `ftw_security_question`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `query_str` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ftw_security_question
-- ----------------------------

-- ----------------------------
-- Table structure for images
-- ----------------------------
DROP TABLE IF EXISTS `images`;
CREATE TABLE `images`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `folder` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `img` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `created_at` datetime NOT NULL,
  `deleted_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of images
-- ----------------------------

-- ----------------------------
-- Table structure for images_screenshots
-- ----------------------------
DROP TABLE IF EXISTS `images_screenshots`;
CREATE TABLE `images_screenshots`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `imgid` int(10) UNSIGNED NULL DEFAULT 0,
  `order` int(11) NULL DEFAULT 999,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of images_screenshots
-- ----------------------------

-- ----------------------------
-- Table structure for log_activity
-- ----------------------------
DROP TABLE IF EXISTS `log_activity`;
CREATE TABLE `log_activity`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `UserId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Type` smallint(5) UNSIGNED NOT NULL DEFAULT 0,
  `IdAddress` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT 'Unknown',
  `Json` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Timestamp` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of log_activity
-- ----------------------------

-- ----------------------------
-- Table structure for login_records
-- ----------------------------
DROP TABLE IF EXISTS `login_records`;
CREATE TABLE `login_records`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `action` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `time` datetime NOT NULL,
  `ip_address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0.0.0.0',
  `browser` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'Unknown Browser Information',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of login_records
-- ----------------------------

-- ----------------------------
-- Table structure for permission_user
-- ----------------------------
DROP TABLE IF EXISTS `permission_user`;
CREATE TABLE `permission_user`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(4) UNSIGNED NOT NULL,
  `permission_id` int(4) UNSIGNED NOT NULL,
  `value` int(4) UNSIGNED NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of permission_user
-- ----------------------------

-- ----------------------------
-- Table structure for permission_usergroup
-- ----------------------------
DROP TABLE IF EXISTS `permission_usergroup`;
CREATE TABLE `permission_usergroup`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `group_id` int(4) UNSIGNED NOT NULL,
  `permission_id` int(4) UNSIGNED NOT NULL,
  `value` int(4) UNSIGNED NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of permission_usergroup
-- ----------------------------

-- ----------------------------
-- Table structure for profile_information
-- ----------------------------
DROP TABLE IF EXISTS `profile_information`;
CREATE TABLE `profile_information`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `location` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `phone` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `facebook` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `instagram` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `twitter` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `youtube` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `twitch` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `about_me` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `selected_character_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `post_count` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `name`(`name`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of profile_information
-- ----------------------------

-- ----------------------------
-- Table structure for profile_status_updates
-- ----------------------------
DROP TABLE IF EXISTS `profile_status_updates`;
CREATE TABLE `profile_status_updates`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(4) UNSIGNED NOT NULL,
  `type` int(4) UNSIGNED ZEROFILL NOT NULL DEFAULT 0000,
  `reply_from` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `message` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of profile_status_updates
-- ----------------------------

-- ----------------------------
-- Table structure for realm
-- ----------------------------
DROP TABLE IF EXISTS `realm`;
CREATE TABLE `realm`  (
  `RealmID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `AuthorityID` smallint(6) UNSIGNED NOT NULL DEFAULT 1 COMMENT 'Authority level required',
  `GameIPAddress` varchar(45) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '127.0.0.1',
  `RpcIPAddress` varchar(45) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '127.0.0.1',
  `GamePort` int(10) UNSIGNED NOT NULL DEFAULT 5816,
  `RpcPort` int(10) UNSIGNED NOT NULL DEFAULT 5817,
  `Status` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `Username` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT 'test',
  `Password` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT 'test',
  `LastPing` datetime NULL DEFAULT NULL,
  `DatabaseHost` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `DatabaseUser` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `DatabasePass` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `DatabaseSchema` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  PRIMARY KEY (`RealmID`) USING BTREE,
  UNIQUE INDEX `RealmID_UNIQUE`(`RealmID`) USING BTREE,
  UNIQUE INDEX `Name_UNIQUE`(`Name`) USING BTREE,
  INDEX `fk_realm_account_authority_idx`(`AuthorityID`) USING BTREE,
  CONSTRAINT `fk_realm_account_authority` FOREIGN KEY (`AuthorityID`) REFERENCES `account_authority` (`AuthorityID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of realm
-- ----------------------------
INSERT INTO `realm` VALUES (1, 'Dark', 1, '192.168.1.23', '127.0.0.1', 5816, 5817, 1, 'inburst', 'test', '2021-07-13 15:51:41', '127.0.0.1', 'root', '', 'conquer');

-- ----------------------------
-- Table structure for realms_status
-- ----------------------------
DROP TABLE IF EXISTS `realms_status`;
CREATE TABLE `realms_status`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `realm_id` int(4) UNSIGNED NOT NULL,
  `realm_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `old_status` tinyint(1) UNSIGNED NOT NULL,
  `new_status` tinyint(1) UNSIGNED NOT NULL,
  `time` datetime NOT NULL,
  `players_online` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `max_players_online` int(4) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `Realms`(`realm_id`) USING BTREE,
  CONSTRAINT `Realms` FOREIGN KEY (`realm_id`) REFERENCES `realm` (`RealmID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE = InnoDB AUTO_INCREMENT = 1937 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of realms_status
-- ----------------------------
INSERT INTO `realms_status` VALUES (1, 1, 'Dark', 1, 1, '2021-07-13 03:13:50', 0, 50);
INSERT INTO `realms_status` VALUES (8, 1, 'Dark', 1, 1, '2021-07-13 03:36:54', 0, 0);
INSERT INTO `realms_status` VALUES (9, 1, 'Dark', 1, 1, '2021-07-13 03:38:06', 0, 0);
INSERT INTO `realms_status` VALUES (10, 1, 'Dark', 1, 1, '2021-07-13 03:40:06', 0, 0);
INSERT INTO `realms_status` VALUES (11, 1, 'Dark', 1, 1, '2021-07-13 03:40:21', 0, 0);
INSERT INTO `realms_status` VALUES (12, 1, 'Dark', 1, 1, '2021-07-13 03:41:01', 0, 0);
INSERT INTO `realms_status` VALUES (13, 1, 'Dark', 1, 1, '2021-07-13 03:41:15', 0, 0);
INSERT INTO `realms_status` VALUES (14, 1, 'Dark', 1, 1, '2021-07-13 03:41:30', 0, 0);
INSERT INTO `realms_status` VALUES (15, 1, 'Dark', 1, 1, '2021-07-13 03:41:45', 0, 0);
INSERT INTO `realms_status` VALUES (16, 1, 'Dark', 1, 1, '2021-07-13 03:42:00', 0, 0);
INSERT INTO `realms_status` VALUES (17, 1, 'Dark', 1, 1, '2021-07-13 03:42:15', 0, 0);
INSERT INTO `realms_status` VALUES (18, 1, 'Dark', 1, 1, '2021-07-13 03:42:31', 0, 0);
INSERT INTO `realms_status` VALUES (19, 1, 'Dark', 1, 1, '2021-07-13 03:42:46', 0, 0);
INSERT INTO `realms_status` VALUES (20, 1, 'Dark', 1, 1, '2021-07-13 03:43:00', 0, 0);
INSERT INTO `realms_status` VALUES (21, 1, 'Dark', 1, 1, '2021-07-13 03:43:27', 0, 0);
INSERT INTO `realms_status` VALUES (22, 1, 'Dark', 1, 1, '2021-07-13 03:44:17', 0, 0);
INSERT INTO `realms_status` VALUES (23, 1, 'Dark', 1, 1, '2021-07-13 03:44:32', 0, 0);
INSERT INTO `realms_status` VALUES (24, 1, 'Dark', 1, 1, '2021-07-13 03:44:47', 0, 0);
INSERT INTO `realms_status` VALUES (25, 1, 'Dark', 1, 1, '2021-07-13 03:45:36', 0, 0);
INSERT INTO `realms_status` VALUES (26, 1, 'Dark', 1, 1, '2021-07-13 03:45:52', 0, 0);
INSERT INTO `realms_status` VALUES (27, 1, 'Dark', 1, 1, '2021-07-13 03:46:07', 0, 0);
INSERT INTO `realms_status` VALUES (28, 1, 'Dark', 1, 1, '2021-07-13 03:46:21', 0, 0);
INSERT INTO `realms_status` VALUES (29, 1, 'Dark', 1, 1, '2021-07-13 03:46:36', 0, 0);
INSERT INTO `realms_status` VALUES (30, 1, 'Dark', 1, 1, '2021-07-13 03:46:51', 0, 0);
INSERT INTO `realms_status` VALUES (31, 1, 'Dark', 1, 1, '2021-07-13 03:48:12', 0, 0);
INSERT INTO `realms_status` VALUES (32, 1, 'Dark', 1, 1, '2021-07-13 03:48:27', 0, 0);
INSERT INTO `realms_status` VALUES (33, 1, 'Dark', 1, 1, '2021-07-13 03:49:04', 0, 0);
INSERT INTO `realms_status` VALUES (34, 1, 'Dark', 1, 1, '2021-07-13 03:49:18', 0, 0);
INSERT INTO `realms_status` VALUES (35, 1, 'Dark', 1, 1, '2021-07-13 03:49:54', 0, 0);
INSERT INTO `realms_status` VALUES (36, 1, 'Dark', 1, 1, '2021-07-13 03:50:09', 0, 0);
INSERT INTO `realms_status` VALUES (37, 1, 'Dark', 1, 1, '2021-07-13 03:50:23', 0, 0);
INSERT INTO `realms_status` VALUES (38, 1, 'Dark', 1, 1, '2021-07-13 03:50:38', 0, 0);
INSERT INTO `realms_status` VALUES (39, 1, 'Dark', 1, 1, '2021-07-13 03:50:53', 0, 0);
INSERT INTO `realms_status` VALUES (40, 1, 'Dark', 1, 1, '2021-07-13 03:51:08', 0, 0);
INSERT INTO `realms_status` VALUES (41, 1, 'Dark', 1, 1, '2021-07-13 03:51:48', 0, 0);
INSERT INTO `realms_status` VALUES (42, 1, 'Dark', 1, 1, '2021-07-13 03:52:03', 0, 0);
INSERT INTO `realms_status` VALUES (43, 1, 'Dark', 1, 1, '2021-07-13 03:52:18', 0, 0);
INSERT INTO `realms_status` VALUES (44, 1, 'Dark', 1, 1, '2021-07-13 03:52:33', 0, 0);
INSERT INTO `realms_status` VALUES (45, 1, 'Dark', 1, 1, '2021-07-13 03:52:49', 0, 0);
INSERT INTO `realms_status` VALUES (46, 1, 'Dark', 1, 1, '2021-07-13 03:53:32', 0, 0);
INSERT INTO `realms_status` VALUES (47, 1, 'Dark', 1, 1, '2021-07-13 03:56:30', 0, 0);
INSERT INTO `realms_status` VALUES (48, 1, 'Dark', 1, 1, '2021-07-13 03:56:46', 0, 0);
INSERT INTO `realms_status` VALUES (49, 1, 'Dark', 1, 1, '2021-07-13 03:57:01', 0, 0);
INSERT INTO `realms_status` VALUES (50, 1, 'Dark', 1, 1, '2021-07-13 03:57:15', 0, 0);
INSERT INTO `realms_status` VALUES (51, 1, 'Dark', 1, 1, '2021-07-13 03:57:30', 0, 0);
INSERT INTO `realms_status` VALUES (52, 1, 'Dark', 1, 1, '2021-07-13 03:57:45', 0, 0);
INSERT INTO `realms_status` VALUES (53, 1, 'Dark', 1, 1, '2021-07-13 03:58:00', 0, 0);
INSERT INTO `realms_status` VALUES (54, 1, 'Dark', 1, 1, '2021-07-13 03:58:15', 0, 0);
INSERT INTO `realms_status` VALUES (55, 1, 'Dark', 1, 1, '2021-07-13 03:58:31', 0, 0);
INSERT INTO `realms_status` VALUES (56, 1, 'Dark', 1, 1, '2021-07-13 03:59:03', 0, 0);
INSERT INTO `realms_status` VALUES (57, 1, 'Dark', 1, 1, '2021-07-13 03:59:18', 0, 0);
INSERT INTO `realms_status` VALUES (58, 1, 'Dark', 1, 1, '2021-07-13 03:59:33', 0, 0);
INSERT INTO `realms_status` VALUES (59, 1, 'Dark', 1, 1, '2021-07-13 03:59:48', 0, 0);
INSERT INTO `realms_status` VALUES (60, 1, 'Dark', 1, 1, '2021-07-13 04:00:04', 0, 0);
INSERT INTO `realms_status` VALUES (61, 1, 'Dark', 1, 1, '2021-07-13 04:00:44', 0, 0);
INSERT INTO `realms_status` VALUES (62, 1, 'Dark', 1, 1, '2021-07-13 04:00:59', 0, 0);
INSERT INTO `realms_status` VALUES (63, 1, 'Dark', 1, 1, '2021-07-13 04:01:14', 0, 0);
INSERT INTO `realms_status` VALUES (64, 1, 'Dark', 1, 1, '2021-07-13 04:01:29', 0, 0);
INSERT INTO `realms_status` VALUES (65, 1, 'Dark', 1, 1, '2021-07-13 04:01:45', 0, 0);
INSERT INTO `realms_status` VALUES (66, 1, 'Dark', 1, 1, '2021-07-13 04:02:16', 0, 0);
INSERT INTO `realms_status` VALUES (67, 1, 'Dark', 1, 1, '2021-07-13 04:02:31', 1, 1);
INSERT INTO `realms_status` VALUES (68, 1, 'Dark', 1, 1, '2021-07-13 04:02:46', 1, 1);
INSERT INTO `realms_status` VALUES (69, 1, 'Dark', 1, 1, '2021-07-13 04:03:02', 1, 1);
INSERT INTO `realms_status` VALUES (70, 1, 'Dark', 1, 1, '2021-07-13 04:03:16', 1, 1);
INSERT INTO `realms_status` VALUES (71, 1, 'Dark', 1, 1, '2021-07-13 04:03:31', 1, 1);
INSERT INTO `realms_status` VALUES (72, 1, 'Dark', 1, 1, '2021-07-13 04:03:46', 1, 1);
INSERT INTO `realms_status` VALUES (73, 1, 'Dark', 1, 1, '2021-07-13 04:04:01', 1, 1);
INSERT INTO `realms_status` VALUES (74, 1, 'Dark', 1, 1, '2021-07-13 04:04:16', 1, 1);
INSERT INTO `realms_status` VALUES (75, 1, 'Dark', 1, 1, '2021-07-13 04:04:32', 1, 1);
INSERT INTO `realms_status` VALUES (76, 1, 'Dark', 1, 1, '2021-07-13 04:04:47', 1, 1);
INSERT INTO `realms_status` VALUES (77, 1, 'Dark', 1, 1, '2021-07-13 04:05:01', 1, 1);
INSERT INTO `realms_status` VALUES (78, 1, 'Dark', 1, 1, '2021-07-13 04:05:16', 1, 1);
INSERT INTO `realms_status` VALUES (79, 1, 'Dark', 1, 1, '2021-07-13 04:05:31', 1, 1);
INSERT INTO `realms_status` VALUES (80, 1, 'Dark', 1, 1, '2021-07-13 04:05:46', 1, 1);
INSERT INTO `realms_status` VALUES (81, 1, 'Dark', 1, 1, '2021-07-13 04:06:01', 1, 1);
INSERT INTO `realms_status` VALUES (82, 1, 'Dark', 1, 1, '2021-07-13 04:06:17', 1, 1);
INSERT INTO `realms_status` VALUES (83, 1, 'Dark', 1, 1, '2021-07-13 04:06:52', 1, 1);
INSERT INTO `realms_status` VALUES (84, 1, 'Dark', 1, 1, '2021-07-13 04:07:07', 1, 1);
INSERT INTO `realms_status` VALUES (85, 1, 'Dark', 1, 1, '2021-07-13 04:07:22', 1, 1);
INSERT INTO `realms_status` VALUES (86, 1, 'Dark', 1, 1, '2021-07-13 04:07:37', 1, 1);
INSERT INTO `realms_status` VALUES (87, 1, 'Dark', 1, 1, '2021-07-13 04:07:52', 1, 1);
INSERT INTO `realms_status` VALUES (88, 1, 'Dark', 1, 1, '2021-07-13 04:08:07', 1, 1);
INSERT INTO `realms_status` VALUES (89, 1, 'Dark', 1, 1, '2021-07-13 04:08:22', 1, 1);
INSERT INTO `realms_status` VALUES (90, 1, 'Dark', 1, 1, '2021-07-13 04:08:37', 1, 1);
INSERT INTO `realms_status` VALUES (91, 1, 'Dark', 1, 1, '2021-07-13 04:08:52', 1, 1);
INSERT INTO `realms_status` VALUES (92, 1, 'Dark', 1, 1, '2021-07-13 04:09:08', 1, 1);
INSERT INTO `realms_status` VALUES (93, 1, 'Dark', 1, 1, '2021-07-13 04:09:23', 1, 1);
INSERT INTO `realms_status` VALUES (94, 1, 'Dark', 1, 1, '2021-07-13 04:09:50', 0, 0);
INSERT INTO `realms_status` VALUES (95, 1, 'Dark', 1, 1, '2021-07-13 04:10:05', 0, 0);
INSERT INTO `realms_status` VALUES (96, 1, 'Dark', 1, 1, '2021-07-13 04:10:20', 0, 0);
INSERT INTO `realms_status` VALUES (97, 1, 'Dark', 1, 1, '2021-07-13 04:10:35', 1, 1);
INSERT INTO `realms_status` VALUES (98, 1, 'Dark', 1, 1, '2021-07-13 04:10:50', 1, 1);
INSERT INTO `realms_status` VALUES (99, 1, 'Dark', 1, 1, '2021-07-13 04:11:06', 1, 1);
INSERT INTO `realms_status` VALUES (100, 1, 'Dark', 1, 1, '2021-07-13 04:11:21', 1, 1);
INSERT INTO `realms_status` VALUES (101, 1, 'Dark', 1, 1, '2021-07-13 04:11:35', 1, 1);
INSERT INTO `realms_status` VALUES (102, 1, 'Dark', 1, 1, '2021-07-13 04:11:50', 1, 1);
INSERT INTO `realms_status` VALUES (103, 1, 'Dark', 1, 1, '2021-07-13 04:12:05', 1, 1);
INSERT INTO `realms_status` VALUES (104, 1, 'Dark', 1, 1, '2021-07-13 04:12:20', 1, 1);
INSERT INTO `realms_status` VALUES (105, 1, 'Dark', 1, 1, '2021-07-13 04:14:36', 0, 0);
INSERT INTO `realms_status` VALUES (106, 1, 'Dark', 1, 1, '2021-07-13 04:14:51', 0, 1);
INSERT INTO `realms_status` VALUES (107, 1, 'Dark', 1, 1, '2021-07-13 04:15:06', 0, 1);
INSERT INTO `realms_status` VALUES (108, 1, 'Dark', 1, 1, '2021-07-13 04:16:03', 0, 0);
INSERT INTO `realms_status` VALUES (109, 1, 'Dark', 1, 1, '2021-07-13 04:19:48', 0, 0);
INSERT INTO `realms_status` VALUES (110, 1, 'Dark', 1, 1, '2021-07-13 04:20:34', 1, 1);
INSERT INTO `realms_status` VALUES (111, 1, 'Dark', 1, 1, '2021-07-13 04:20:34', 1, 1);
INSERT INTO `realms_status` VALUES (112, 1, 'Dark', 1, 1, '2021-07-13 04:20:34', 1, 1);
INSERT INTO `realms_status` VALUES (113, 1, 'Dark', 1, 1, '2021-07-13 04:26:45', 0, 0);
INSERT INTO `realms_status` VALUES (114, 1, 'Dark', 1, 1, '2021-07-13 04:34:47', 0, 0);
INSERT INTO `realms_status` VALUES (115, 1, 'Dark', 1, 1, '2021-07-13 04:39:41', 0, 0);
INSERT INTO `realms_status` VALUES (116, 1, 'Dark', 1, 1, '2021-07-13 04:39:56', 0, 0);
INSERT INTO `realms_status` VALUES (117, 1, 'Dark', 1, 1, '2021-07-13 04:40:11', 0, 0);
INSERT INTO `realms_status` VALUES (118, 1, 'Dark', 1, 1, '2021-07-13 04:40:26', 0, 0);
INSERT INTO `realms_status` VALUES (119, 1, 'Dark', 1, 1, '2021-07-13 04:40:41', 0, 0);
INSERT INTO `realms_status` VALUES (120, 1, 'Dark', 1, 1, '2021-07-13 04:40:57', 0, 0);
INSERT INTO `realms_status` VALUES (121, 1, 'Dark', 1, 1, '2021-07-13 04:41:12', 0, 0);
INSERT INTO `realms_status` VALUES (122, 1, 'Dark', 1, 1, '2021-07-13 04:41:26', 0, 0);
INSERT INTO `realms_status` VALUES (123, 1, 'Dark', 1, 1, '2021-07-13 04:41:41', 0, 0);
INSERT INTO `realms_status` VALUES (124, 1, 'Dark', 1, 1, '2021-07-13 04:41:56', 0, 0);
INSERT INTO `realms_status` VALUES (125, 1, 'Dark', 1, 1, '2021-07-13 04:42:11', 0, 0);
INSERT INTO `realms_status` VALUES (126, 1, 'Dark', 1, 1, '2021-07-13 04:42:26', 0, 0);
INSERT INTO `realms_status` VALUES (127, 1, 'Dark', 1, 1, '2021-07-13 04:42:42', 0, 0);
INSERT INTO `realms_status` VALUES (128, 1, 'Dark', 1, 1, '2021-07-13 04:42:56', 0, 0);
INSERT INTO `realms_status` VALUES (129, 1, 'Dark', 1, 1, '2021-07-13 04:43:11', 0, 0);
INSERT INTO `realms_status` VALUES (130, 1, 'Dark', 1, 1, '2021-07-13 04:43:26', 0, 0);
INSERT INTO `realms_status` VALUES (131, 1, 'Dark', 1, 1, '2021-07-13 04:43:41', 0, 0);
INSERT INTO `realms_status` VALUES (132, 1, 'Dark', 1, 1, '2021-07-13 04:43:56', 0, 0);
INSERT INTO `realms_status` VALUES (133, 1, 'Dark', 1, 1, '2021-07-13 04:44:12', 0, 0);
INSERT INTO `realms_status` VALUES (134, 1, 'Dark', 1, 1, '2021-07-13 04:44:27', 0, 0);
INSERT INTO `realms_status` VALUES (135, 1, 'Dark', 1, 1, '2021-07-13 04:44:41', 0, 0);
INSERT INTO `realms_status` VALUES (136, 1, 'Dark', 1, 1, '2021-07-13 04:44:58', 1, 1);
INSERT INTO `realms_status` VALUES (137, 1, 'Dark', 1, 1, '2021-07-13 04:45:14', 1, 1);
INSERT INTO `realms_status` VALUES (138, 1, 'Dark', 1, 1, '2021-07-13 04:55:46', 0, 0);
INSERT INTO `realms_status` VALUES (139, 1, 'Dark', 1, 1, '2021-07-13 04:56:01', 1, 1);
INSERT INTO `realms_status` VALUES (140, 1, 'Dark', 1, 1, '2021-07-13 04:56:16', 1, 1);
INSERT INTO `realms_status` VALUES (141, 1, 'Dark', 1, 1, '2021-07-13 04:56:32', 1, 1);
INSERT INTO `realms_status` VALUES (142, 1, 'Dark', 1, 1, '2021-07-13 04:56:47', 1, 1);
INSERT INTO `realms_status` VALUES (143, 1, 'Dark', 1, 1, '2021-07-13 04:57:01', 1, 1);
INSERT INTO `realms_status` VALUES (144, 1, 'Dark', 1, 1, '2021-07-13 04:57:16', 1, 1);
INSERT INTO `realms_status` VALUES (145, 1, 'Dark', 1, 1, '2021-07-13 04:57:31', 1, 1);
INSERT INTO `realms_status` VALUES (146, 1, 'Dark', 1, 1, '2021-07-13 04:57:46', 1, 1);
INSERT INTO `realms_status` VALUES (147, 1, 'Dark', 1, 1, '2021-07-13 04:58:01', 1, 1);
INSERT INTO `realms_status` VALUES (148, 1, 'Dark', 1, 1, '2021-07-13 04:58:17', 1, 1);
INSERT INTO `realms_status` VALUES (149, 1, 'Dark', 1, 1, '2021-07-13 04:58:32', 1, 1);
INSERT INTO `realms_status` VALUES (150, 1, 'Dark', 1, 1, '2021-07-13 04:58:46', 1, 1);
INSERT INTO `realms_status` VALUES (151, 1, 'Dark', 1, 1, '2021-07-13 04:59:01', 1, 1);
INSERT INTO `realms_status` VALUES (152, 1, 'Dark', 1, 1, '2021-07-13 04:59:16', 1, 1);
INSERT INTO `realms_status` VALUES (153, 1, 'Dark', 1, 1, '2021-07-13 04:59:31', 1, 1);
INSERT INTO `realms_status` VALUES (154, 1, 'Dark', 1, 1, '2021-07-13 04:59:46', 1, 1);
INSERT INTO `realms_status` VALUES (155, 1, 'Dark', 1, 1, '2021-07-13 05:00:01', 1, 1);
INSERT INTO `realms_status` VALUES (156, 1, 'Dark', 1, 1, '2021-07-13 05:00:17', 1, 1);
INSERT INTO `realms_status` VALUES (157, 1, 'Dark', 1, 1, '2021-07-13 05:00:32', 1, 1);
INSERT INTO `realms_status` VALUES (158, 1, 'Dark', 1, 1, '2021-07-13 05:00:47', 1, 1);
INSERT INTO `realms_status` VALUES (159, 1, 'Dark', 1, 1, '2021-07-13 05:01:02', 1, 1);
INSERT INTO `realms_status` VALUES (160, 1, 'Dark', 1, 1, '2021-07-13 05:01:17', 1, 1);
INSERT INTO `realms_status` VALUES (161, 1, 'Dark', 1, 1, '2021-07-13 05:01:44', 0, 0);
INSERT INTO `realms_status` VALUES (162, 1, 'Dark', 1, 1, '2021-07-13 05:02:01', 1, 1);
INSERT INTO `realms_status` VALUES (163, 1, 'Dark', 1, 1, '2021-07-13 05:02:14', 1, 1);
INSERT INTO `realms_status` VALUES (164, 1, 'Dark', 1, 1, '2021-07-13 05:02:28', 1, 1);
INSERT INTO `realms_status` VALUES (165, 1, 'Dark', 1, 1, '2021-07-13 05:02:43', 1, 1);
INSERT INTO `realms_status` VALUES (166, 1, 'Dark', 1, 1, '2021-07-13 05:02:58', 1, 1);
INSERT INTO `realms_status` VALUES (167, 1, 'Dark', 1, 1, '2021-07-13 05:03:14', 1, 1);
INSERT INTO `realms_status` VALUES (168, 1, 'Dark', 1, 1, '2021-07-13 05:03:29', 1, 1);
INSERT INTO `realms_status` VALUES (169, 1, 'Dark', 1, 1, '2021-07-13 05:03:43', 1, 1);
INSERT INTO `realms_status` VALUES (170, 1, 'Dark', 1, 1, '2021-07-13 05:03:58', 1, 1);
INSERT INTO `realms_status` VALUES (171, 1, 'Dark', 1, 1, '2021-07-13 05:04:13', 1, 1);
INSERT INTO `realms_status` VALUES (172, 1, 'Dark', 1, 1, '2021-07-13 05:04:28', 1, 1);
INSERT INTO `realms_status` VALUES (173, 1, 'Dark', 1, 1, '2021-07-13 05:04:43', 1, 1);
INSERT INTO `realms_status` VALUES (174, 1, 'Dark', 1, 1, '2021-07-13 05:04:59', 1, 1);
INSERT INTO `realms_status` VALUES (175, 1, 'Dark', 1, 1, '2021-07-13 05:05:14', 1, 1);
INSERT INTO `realms_status` VALUES (176, 1, 'Dark', 1, 1, '2021-07-13 05:05:28', 1, 1);
INSERT INTO `realms_status` VALUES (177, 1, 'Dark', 1, 1, '2021-07-13 05:05:43', 1, 1);
INSERT INTO `realms_status` VALUES (178, 1, 'Dark', 1, 1, '2021-07-13 05:05:58', 1, 1);
INSERT INTO `realms_status` VALUES (179, 1, 'Dark', 1, 1, '2021-07-13 05:06:13', 1, 1);
INSERT INTO `realms_status` VALUES (180, 1, 'Dark', 1, 1, '2021-07-13 05:06:28', 1, 1);
INSERT INTO `realms_status` VALUES (181, 1, 'Dark', 1, 1, '2021-07-13 05:06:44', 1, 1);
INSERT INTO `realms_status` VALUES (182, 1, 'Dark', 1, 1, '2021-07-13 05:06:59', 1, 1);
INSERT INTO `realms_status` VALUES (183, 1, 'Dark', 1, 1, '2021-07-13 05:07:13', 1, 1);
INSERT INTO `realms_status` VALUES (184, 1, 'Dark', 1, 1, '2021-07-13 05:07:28', 1, 1);
INSERT INTO `realms_status` VALUES (185, 1, 'Dark', 1, 1, '2021-07-13 05:07:43', 1, 1);
INSERT INTO `realms_status` VALUES (186, 1, 'Dark', 1, 1, '2021-07-13 05:07:58', 1, 1);
INSERT INTO `realms_status` VALUES (187, 1, 'Dark', 1, 1, '2021-07-13 05:08:13', 1, 1);
INSERT INTO `realms_status` VALUES (188, 1, 'Dark', 1, 1, '2021-07-13 05:08:28', 1, 1);
INSERT INTO `realms_status` VALUES (189, 1, 'Dark', 1, 1, '2021-07-13 05:08:44', 1, 1);
INSERT INTO `realms_status` VALUES (190, 1, 'Dark', 1, 1, '2021-07-13 05:08:59', 1, 1);
INSERT INTO `realms_status` VALUES (191, 1, 'Dark', 1, 1, '2021-07-13 05:09:14', 1, 1);
INSERT INTO `realms_status` VALUES (192, 1, 'Dark', 1, 1, '2021-07-13 05:09:29', 1, 1);
INSERT INTO `realms_status` VALUES (193, 1, 'Dark', 1, 1, '2021-07-13 05:09:44', 1, 1);
INSERT INTO `realms_status` VALUES (194, 1, 'Dark', 1, 1, '2021-07-13 05:10:35', 0, 0);
INSERT INTO `realms_status` VALUES (195, 1, 'Dark', 1, 1, '2021-07-13 05:10:50', 0, 0);
INSERT INTO `realms_status` VALUES (196, 1, 'Dark', 1, 1, '2021-07-13 05:11:21', 1, 1);
INSERT INTO `realms_status` VALUES (197, 1, 'Dark', 1, 1, '2021-07-13 05:11:21', 1, 1);
INSERT INTO `realms_status` VALUES (198, 1, 'Dark', 1, 1, '2021-07-13 05:11:34', 1, 1);
INSERT INTO `realms_status` VALUES (199, 1, 'Dark', 1, 1, '2021-07-13 05:11:49', 1, 1);
INSERT INTO `realms_status` VALUES (200, 1, 'Dark', 1, 1, '2021-07-13 05:12:04', 1, 1);
INSERT INTO `realms_status` VALUES (201, 1, 'Dark', 1, 1, '2021-07-13 05:12:20', 1, 1);
INSERT INTO `realms_status` VALUES (202, 1, 'Dark', 1, 1, '2021-07-13 05:12:34', 1, 1);
INSERT INTO `realms_status` VALUES (203, 1, 'Dark', 1, 1, '2021-07-13 05:12:49', 1, 1);
INSERT INTO `realms_status` VALUES (204, 1, 'Dark', 1, 1, '2021-07-13 05:13:04', 1, 1);
INSERT INTO `realms_status` VALUES (205, 1, 'Dark', 1, 1, '2021-07-13 05:13:19', 1, 1);
INSERT INTO `realms_status` VALUES (206, 1, 'Dark', 1, 1, '2021-07-13 05:14:31', 1, 1);
INSERT INTO `realms_status` VALUES (207, 1, 'Dark', 1, 1, '2021-07-13 05:23:59', 1, 1);
INSERT INTO `realms_status` VALUES (208, 1, 'Dark', 1, 1, '2021-07-13 05:25:52', 0, 0);
INSERT INTO `realms_status` VALUES (209, 1, 'Dark', 1, 1, '2021-07-13 05:26:06', 0, 0);
INSERT INTO `realms_status` VALUES (210, 1, 'Dark', 1, 1, '2021-07-13 05:26:21', 1, 1);
INSERT INTO `realms_status` VALUES (211, 1, 'Dark', 1, 1, '2021-07-13 05:27:46', 0, 0);
INSERT INTO `realms_status` VALUES (212, 1, 'Dark', 1, 1, '2021-07-13 05:34:37', 1, 1);
INSERT INTO `realms_status` VALUES (213, 1, 'Dark', 1, 1, '2021-07-13 05:34:51', 1, 1);
INSERT INTO `realms_status` VALUES (214, 1, 'Dark', 1, 1, '2021-07-13 05:35:06', 1, 1);
INSERT INTO `realms_status` VALUES (215, 1, 'Dark', 1, 1, '2021-07-13 05:35:21', 1, 1);
INSERT INTO `realms_status` VALUES (216, 1, 'Dark', 1, 1, '2021-07-13 05:35:36', 1, 1);
INSERT INTO `realms_status` VALUES (217, 1, 'Dark', 1, 1, '2021-07-13 05:35:51', 1, 1);
INSERT INTO `realms_status` VALUES (218, 1, 'Dark', 1, 1, '2021-07-13 05:36:07', 1, 1);
INSERT INTO `realms_status` VALUES (219, 1, 'Dark', 1, 1, '2021-07-13 05:36:22', 1, 1);
INSERT INTO `realms_status` VALUES (220, 1, 'Dark', 1, 1, '2021-07-13 05:36:36', 1, 1);
INSERT INTO `realms_status` VALUES (221, 1, 'Dark', 1, 1, '2021-07-13 05:36:51', 1, 1);
INSERT INTO `realms_status` VALUES (222, 1, 'Dark', 1, 1, '2021-07-13 05:37:06', 1, 1);
INSERT INTO `realms_status` VALUES (223, 1, 'Dark', 1, 1, '2021-07-13 05:37:21', 1, 1);
INSERT INTO `realms_status` VALUES (224, 1, 'Dark', 1, 1, '2021-07-13 05:37:36', 1, 1);
INSERT INTO `realms_status` VALUES (225, 1, 'Dark', 1, 1, '2021-07-13 05:37:52', 1, 1);
INSERT INTO `realms_status` VALUES (226, 1, 'Dark', 1, 1, '2021-07-13 05:38:07', 1, 1);
INSERT INTO `realms_status` VALUES (227, 1, 'Dark', 1, 1, '2021-07-13 05:38:21', 1, 1);
INSERT INTO `realms_status` VALUES (228, 1, 'Dark', 1, 1, '2021-07-13 05:38:36', 1, 1);
INSERT INTO `realms_status` VALUES (229, 1, 'Dark', 1, 1, '2021-07-13 05:38:51', 1, 1);
INSERT INTO `realms_status` VALUES (230, 1, 'Dark', 1, 1, '2021-07-13 05:39:06', 1, 1);
INSERT INTO `realms_status` VALUES (231, 1, 'Dark', 1, 1, '2021-07-13 05:39:22', 1, 1);
INSERT INTO `realms_status` VALUES (232, 1, 'Dark', 1, 1, '2021-07-13 05:39:37', 1, 1);
INSERT INTO `realms_status` VALUES (233, 1, 'Dark', 1, 1, '2021-07-13 05:39:51', 1, 1);
INSERT INTO `realms_status` VALUES (234, 1, 'Dark', 1, 1, '2021-07-13 05:40:06', 1, 1);
INSERT INTO `realms_status` VALUES (235, 1, 'Dark', 1, 1, '2021-07-13 05:40:21', 1, 1);
INSERT INTO `realms_status` VALUES (236, 1, 'Dark', 1, 1, '2021-07-13 05:40:36', 1, 1);
INSERT INTO `realms_status` VALUES (237, 1, 'Dark', 1, 1, '2021-07-13 05:40:51', 1, 1);
INSERT INTO `realms_status` VALUES (238, 1, 'Dark', 1, 1, '2021-07-13 05:41:07', 1, 1);
INSERT INTO `realms_status` VALUES (239, 1, 'Dark', 1, 1, '2021-07-13 05:41:22', 1, 1);
INSERT INTO `realms_status` VALUES (240, 1, 'Dark', 1, 1, '2021-07-13 05:41:36', 1, 1);
INSERT INTO `realms_status` VALUES (241, 1, 'Dark', 1, 1, '2021-07-13 05:41:51', 1, 1);
INSERT INTO `realms_status` VALUES (242, 1, 'Dark', 1, 1, '2021-07-13 05:42:06', 1, 1);
INSERT INTO `realms_status` VALUES (243, 1, 'Dark', 1, 1, '2021-07-13 05:42:21', 1, 1);
INSERT INTO `realms_status` VALUES (244, 1, 'Dark', 1, 1, '2021-07-13 05:42:36', 1, 1);
INSERT INTO `realms_status` VALUES (245, 1, 'Dark', 1, 1, '2021-07-13 05:42:51', 1, 1);
INSERT INTO `realms_status` VALUES (246, 1, 'Dark', 1, 1, '2021-07-13 05:43:07', 1, 1);
INSERT INTO `realms_status` VALUES (247, 1, 'Dark', 1, 1, '2021-07-13 05:43:22', 1, 1);
INSERT INTO `realms_status` VALUES (248, 1, 'Dark', 1, 1, '2021-07-13 05:43:36', 1, 1);
INSERT INTO `realms_status` VALUES (249, 1, 'Dark', 1, 1, '2021-07-13 05:43:51', 1, 1);
INSERT INTO `realms_status` VALUES (250, 1, 'Dark', 1, 1, '2021-07-13 05:44:06', 1, 1);
INSERT INTO `realms_status` VALUES (251, 1, 'Dark', 1, 1, '2021-07-13 05:44:21', 1, 1);
INSERT INTO `realms_status` VALUES (252, 1, 'Dark', 1, 1, '2021-07-13 05:44:36', 1, 1);
INSERT INTO `realms_status` VALUES (253, 1, 'Dark', 1, 1, '2021-07-13 05:44:52', 1, 1);
INSERT INTO `realms_status` VALUES (254, 1, 'Dark', 1, 1, '2021-07-13 05:45:07', 1, 1);
INSERT INTO `realms_status` VALUES (255, 1, 'Dark', 1, 1, '2021-07-13 05:45:21', 1, 1);
INSERT INTO `realms_status` VALUES (256, 1, 'Dark', 1, 1, '2021-07-13 05:45:36', 1, 1);
INSERT INTO `realms_status` VALUES (257, 1, 'Dark', 1, 1, '2021-07-13 05:45:51', 1, 1);
INSERT INTO `realms_status` VALUES (258, 1, 'Dark', 1, 1, '2021-07-13 05:46:06', 1, 1);
INSERT INTO `realms_status` VALUES (259, 1, 'Dark', 1, 1, '2021-07-13 05:46:21', 1, 1);
INSERT INTO `realms_status` VALUES (260, 1, 'Dark', 1, 1, '2021-07-13 05:46:37', 1, 1);
INSERT INTO `realms_status` VALUES (261, 1, 'Dark', 1, 1, '2021-07-13 05:46:52', 1, 1);
INSERT INTO `realms_status` VALUES (262, 1, 'Dark', 1, 1, '2021-07-13 05:47:06', 1, 1);
INSERT INTO `realms_status` VALUES (263, 1, 'Dark', 1, 1, '2021-07-13 05:47:21', 1, 1);
INSERT INTO `realms_status` VALUES (264, 1, 'Dark', 1, 1, '2021-07-13 05:47:36', 1, 1);
INSERT INTO `realms_status` VALUES (265, 1, 'Dark', 1, 1, '2021-07-13 05:47:51', 1, 1);
INSERT INTO `realms_status` VALUES (266, 1, 'Dark', 1, 1, '2021-07-13 05:48:06', 1, 1);
INSERT INTO `realms_status` VALUES (267, 1, 'Dark', 1, 1, '2021-07-13 05:48:22', 1, 1);
INSERT INTO `realms_status` VALUES (268, 1, 'Dark', 1, 1, '2021-07-13 05:48:37', 1, 1);
INSERT INTO `realms_status` VALUES (269, 1, 'Dark', 1, 1, '2021-07-13 05:48:51', 1, 1);
INSERT INTO `realms_status` VALUES (270, 1, 'Dark', 1, 1, '2021-07-13 05:49:06', 1, 1);
INSERT INTO `realms_status` VALUES (271, 1, 'Dark', 1, 1, '2021-07-13 05:49:21', 1, 1);
INSERT INTO `realms_status` VALUES (272, 1, 'Dark', 1, 1, '2021-07-13 05:49:36', 1, 1);
INSERT INTO `realms_status` VALUES (273, 1, 'Dark', 1, 1, '2021-07-13 05:49:51', 1, 1);
INSERT INTO `realms_status` VALUES (274, 1, 'Dark', 1, 1, '2021-07-13 05:50:07', 1, 1);
INSERT INTO `realms_status` VALUES (275, 1, 'Dark', 1, 1, '2021-07-13 05:50:22', 1, 1);
INSERT INTO `realms_status` VALUES (276, 1, 'Dark', 1, 1, '2021-07-13 05:50:36', 1, 1);
INSERT INTO `realms_status` VALUES (277, 1, 'Dark', 1, 1, '2021-07-13 05:50:51', 1, 1);
INSERT INTO `realms_status` VALUES (278, 1, 'Dark', 1, 1, '2021-07-13 05:51:06', 1, 1);
INSERT INTO `realms_status` VALUES (279, 1, 'Dark', 1, 1, '2021-07-13 05:51:21', 1, 1);
INSERT INTO `realms_status` VALUES (280, 1, 'Dark', 1, 1, '2021-07-13 05:51:36', 1, 1);
INSERT INTO `realms_status` VALUES (281, 1, 'Dark', 1, 1, '2021-07-13 05:51:51', 1, 1);
INSERT INTO `realms_status` VALUES (282, 1, 'Dark', 1, 1, '2021-07-13 05:52:07', 1, 1);
INSERT INTO `realms_status` VALUES (283, 1, 'Dark', 1, 1, '2021-07-13 05:52:21', 1, 1);
INSERT INTO `realms_status` VALUES (284, 1, 'Dark', 1, 1, '2021-07-13 05:52:36', 1, 1);
INSERT INTO `realms_status` VALUES (285, 1, 'Dark', 1, 1, '2021-07-13 05:52:51', 1, 1);
INSERT INTO `realms_status` VALUES (286, 1, 'Dark', 1, 1, '2021-07-13 05:53:06', 1, 1);
INSERT INTO `realms_status` VALUES (287, 1, 'Dark', 1, 1, '2021-07-13 05:53:21', 1, 1);
INSERT INTO `realms_status` VALUES (288, 1, 'Dark', 1, 1, '2021-07-13 05:53:36', 1, 1);
INSERT INTO `realms_status` VALUES (289, 1, 'Dark', 1, 1, '2021-07-13 05:53:52', 1, 1);
INSERT INTO `realms_status` VALUES (290, 1, 'Dark', 1, 1, '2021-07-13 05:54:06', 1, 1);
INSERT INTO `realms_status` VALUES (291, 1, 'Dark', 1, 1, '2021-07-13 05:54:21', 1, 1);
INSERT INTO `realms_status` VALUES (292, 1, 'Dark', 1, 1, '2021-07-13 05:54:36', 1, 1);
INSERT INTO `realms_status` VALUES (293, 1, 'Dark', 1, 1, '2021-07-13 05:54:51', 1, 1);
INSERT INTO `realms_status` VALUES (294, 1, 'Dark', 1, 1, '2021-07-13 05:55:06', 1, 1);
INSERT INTO `realms_status` VALUES (295, 1, 'Dark', 1, 1, '2021-07-13 05:55:22', 1, 1);
INSERT INTO `realms_status` VALUES (296, 1, 'Dark', 1, 1, '2021-07-13 05:55:37', 1, 1);
INSERT INTO `realms_status` VALUES (297, 1, 'Dark', 1, 1, '2021-07-13 05:55:52', 1, 1);
INSERT INTO `realms_status` VALUES (298, 1, 'Dark', 1, 1, '2021-07-13 05:56:07', 1, 1);
INSERT INTO `realms_status` VALUES (299, 1, 'Dark', 1, 1, '2021-07-13 05:56:22', 1, 1);
INSERT INTO `realms_status` VALUES (300, 1, 'Dark', 1, 1, '2021-07-13 05:56:37', 1, 1);
INSERT INTO `realms_status` VALUES (301, 1, 'Dark', 1, 1, '2021-07-13 05:56:52', 1, 1);
INSERT INTO `realms_status` VALUES (302, 1, 'Dark', 1, 1, '2021-07-13 05:57:07', 1, 1);
INSERT INTO `realms_status` VALUES (303, 1, 'Dark', 1, 1, '2021-07-13 05:57:23', 1, 1);
INSERT INTO `realms_status` VALUES (304, 1, 'Dark', 1, 1, '2021-07-13 05:57:38', 1, 1);
INSERT INTO `realms_status` VALUES (305, 1, 'Dark', 1, 1, '2021-07-13 05:57:53', 1, 1);
INSERT INTO `realms_status` VALUES (306, 1, 'Dark', 1, 1, '2021-07-13 05:58:08', 1, 1);
INSERT INTO `realms_status` VALUES (307, 1, 'Dark', 1, 1, '2021-07-13 05:58:23', 1, 1);
INSERT INTO `realms_status` VALUES (308, 1, 'Dark', 1, 1, '2021-07-13 05:58:38', 1, 1);
INSERT INTO `realms_status` VALUES (309, 1, 'Dark', 1, 1, '2021-07-13 05:58:54', 1, 1);
INSERT INTO `realms_status` VALUES (310, 1, 'Dark', 1, 1, '2021-07-13 05:59:09', 1, 1);
INSERT INTO `realms_status` VALUES (311, 1, 'Dark', 1, 1, '2021-07-13 05:59:23', 1, 1);
INSERT INTO `realms_status` VALUES (312, 1, 'Dark', 1, 1, '2021-07-13 05:59:38', 1, 1);
INSERT INTO `realms_status` VALUES (313, 1, 'Dark', 1, 1, '2021-07-13 05:59:53', 1, 1);
INSERT INTO `realms_status` VALUES (314, 1, 'Dark', 1, 1, '2021-07-13 06:00:08', 1, 1);
INSERT INTO `realms_status` VALUES (315, 1, 'Dark', 1, 1, '2021-07-13 06:00:23', 1, 1);
INSERT INTO `realms_status` VALUES (316, 1, 'Dark', 1, 1, '2021-07-13 06:00:39', 1, 1);
INSERT INTO `realms_status` VALUES (317, 1, 'Dark', 1, 1, '2021-07-13 06:00:54', 1, 1);
INSERT INTO `realms_status` VALUES (318, 1, 'Dark', 1, 1, '2021-07-13 06:01:08', 1, 1);
INSERT INTO `realms_status` VALUES (319, 1, 'Dark', 1, 1, '2021-07-13 06:01:23', 1, 1);
INSERT INTO `realms_status` VALUES (320, 1, 'Dark', 1, 1, '2021-07-13 06:01:38', 1, 1);
INSERT INTO `realms_status` VALUES (321, 1, 'Dark', 1, 1, '2021-07-13 06:01:53', 1, 1);
INSERT INTO `realms_status` VALUES (322, 1, 'Dark', 1, 1, '2021-07-13 06:02:08', 1, 1);
INSERT INTO `realms_status` VALUES (323, 1, 'Dark', 1, 1, '2021-07-13 06:02:24', 1, 1);
INSERT INTO `realms_status` VALUES (324, 1, 'Dark', 1, 1, '2021-07-13 06:02:39', 1, 1);
INSERT INTO `realms_status` VALUES (325, 1, 'Dark', 1, 1, '2021-07-13 06:02:53', 1, 1);
INSERT INTO `realms_status` VALUES (326, 1, 'Dark', 1, 1, '2021-07-13 06:03:08', 1, 1);
INSERT INTO `realms_status` VALUES (327, 1, 'Dark', 1, 1, '2021-07-13 06:03:23', 1, 1);
INSERT INTO `realms_status` VALUES (328, 1, 'Dark', 1, 1, '2021-07-13 06:03:38', 1, 1);
INSERT INTO `realms_status` VALUES (329, 1, 'Dark', 1, 1, '2021-07-13 06:03:53', 1, 1);
INSERT INTO `realms_status` VALUES (330, 1, 'Dark', 1, 1, '2021-07-13 06:04:08', 1, 1);
INSERT INTO `realms_status` VALUES (331, 1, 'Dark', 1, 1, '2021-07-13 06:04:24', 1, 1);
INSERT INTO `realms_status` VALUES (332, 1, 'Dark', 1, 1, '2021-07-13 06:04:38', 1, 1);
INSERT INTO `realms_status` VALUES (333, 1, 'Dark', 1, 1, '2021-07-13 06:04:53', 1, 1);
INSERT INTO `realms_status` VALUES (334, 1, 'Dark', 1, 1, '2021-07-13 06:05:08', 1, 1);
INSERT INTO `realms_status` VALUES (335, 1, 'Dark', 1, 1, '2021-07-13 06:05:23', 1, 1);
INSERT INTO `realms_status` VALUES (336, 1, 'Dark', 1, 1, '2021-07-13 06:05:38', 1, 1);
INSERT INTO `realms_status` VALUES (337, 1, 'Dark', 1, 1, '2021-07-13 06:05:54', 1, 1);
INSERT INTO `realms_status` VALUES (338, 1, 'Dark', 1, 1, '2021-07-13 06:06:09', 1, 1);
INSERT INTO `realms_status` VALUES (339, 1, 'Dark', 1, 1, '2021-07-13 06:06:23', 1, 1);
INSERT INTO `realms_status` VALUES (340, 1, 'Dark', 1, 1, '2021-07-13 06:06:38', 1, 1);
INSERT INTO `realms_status` VALUES (341, 1, 'Dark', 1, 1, '2021-07-13 06:06:53', 1, 1);
INSERT INTO `realms_status` VALUES (342, 1, 'Dark', 1, 1, '2021-07-13 06:07:08', 1, 1);
INSERT INTO `realms_status` VALUES (343, 1, 'Dark', 1, 1, '2021-07-13 06:07:23', 1, 1);
INSERT INTO `realms_status` VALUES (344, 1, 'Dark', 1, 1, '2021-07-13 06:07:38', 1, 1);
INSERT INTO `realms_status` VALUES (345, 1, 'Dark', 1, 1, '2021-07-13 06:07:54', 1, 1);
INSERT INTO `realms_status` VALUES (346, 1, 'Dark', 1, 1, '2021-07-13 06:08:09', 1, 1);
INSERT INTO `realms_status` VALUES (347, 1, 'Dark', 1, 1, '2021-07-13 06:08:23', 1, 1);
INSERT INTO `realms_status` VALUES (348, 1, 'Dark', 1, 1, '2021-07-13 06:08:38', 1, 1);
INSERT INTO `realms_status` VALUES (349, 1, 'Dark', 1, 1, '2021-07-13 06:08:53', 1, 1);
INSERT INTO `realms_status` VALUES (350, 1, 'Dark', 1, 1, '2021-07-13 06:09:08', 1, 1);
INSERT INTO `realms_status` VALUES (351, 1, 'Dark', 1, 1, '2021-07-13 06:09:24', 1, 1);
INSERT INTO `realms_status` VALUES (352, 1, 'Dark', 1, 1, '2021-07-13 06:09:39', 1, 1);
INSERT INTO `realms_status` VALUES (353, 1, 'Dark', 1, 1, '2021-07-13 06:09:53', 1, 1);
INSERT INTO `realms_status` VALUES (354, 1, 'Dark', 1, 1, '2021-07-13 06:10:08', 1, 1);
INSERT INTO `realms_status` VALUES (355, 1, 'Dark', 1, 1, '2021-07-13 06:10:23', 1, 1);
INSERT INTO `realms_status` VALUES (356, 1, 'Dark', 1, 1, '2021-07-13 06:10:38', 1, 1);
INSERT INTO `realms_status` VALUES (357, 1, 'Dark', 1, 1, '2021-07-13 06:10:53', 1, 1);
INSERT INTO `realms_status` VALUES (358, 1, 'Dark', 1, 1, '2021-07-13 06:11:08', 1, 1);
INSERT INTO `realms_status` VALUES (359, 1, 'Dark', 1, 1, '2021-07-13 06:11:24', 1, 1);
INSERT INTO `realms_status` VALUES (360, 1, 'Dark', 1, 1, '2021-07-13 06:11:39', 1, 1);
INSERT INTO `realms_status` VALUES (361, 1, 'Dark', 1, 1, '2021-07-13 06:11:53', 1, 1);
INSERT INTO `realms_status` VALUES (362, 1, 'Dark', 1, 1, '2021-07-13 06:12:08', 1, 1);
INSERT INTO `realms_status` VALUES (363, 1, 'Dark', 1, 1, '2021-07-13 06:12:23', 1, 1);
INSERT INTO `realms_status` VALUES (364, 1, 'Dark', 1, 1, '2021-07-13 06:12:38', 1, 1);
INSERT INTO `realms_status` VALUES (365, 1, 'Dark', 1, 1, '2021-07-13 06:12:53', 1, 1);
INSERT INTO `realms_status` VALUES (366, 1, 'Dark', 1, 1, '2021-07-13 06:13:09', 1, 1);
INSERT INTO `realms_status` VALUES (367, 1, 'Dark', 1, 1, '2021-07-13 06:13:24', 1, 1);
INSERT INTO `realms_status` VALUES (368, 1, 'Dark', 1, 1, '2021-07-13 06:13:38', 1, 1);
INSERT INTO `realms_status` VALUES (369, 1, 'Dark', 1, 1, '2021-07-13 06:13:53', 1, 1);
INSERT INTO `realms_status` VALUES (370, 1, 'Dark', 1, 1, '2021-07-13 06:14:08', 1, 1);
INSERT INTO `realms_status` VALUES (371, 1, 'Dark', 1, 1, '2021-07-13 06:14:23', 1, 1);
INSERT INTO `realms_status` VALUES (372, 1, 'Dark', 1, 1, '2021-07-13 06:14:38', 1, 1);
INSERT INTO `realms_status` VALUES (373, 1, 'Dark', 1, 1, '2021-07-13 06:14:54', 1, 1);
INSERT INTO `realms_status` VALUES (374, 1, 'Dark', 1, 1, '2021-07-13 06:15:09', 1, 1);
INSERT INTO `realms_status` VALUES (375, 1, 'Dark', 1, 1, '2021-07-13 06:15:23', 1, 1);
INSERT INTO `realms_status` VALUES (376, 1, 'Dark', 1, 1, '2021-07-13 06:15:38', 1, 1);
INSERT INTO `realms_status` VALUES (377, 1, 'Dark', 1, 1, '2021-07-13 06:15:53', 1, 1);
INSERT INTO `realms_status` VALUES (378, 1, 'Dark', 1, 1, '2021-07-13 06:16:08', 1, 1);
INSERT INTO `realms_status` VALUES (379, 1, 'Dark', 1, 1, '2021-07-13 06:16:23', 1, 1);
INSERT INTO `realms_status` VALUES (380, 1, 'Dark', 1, 1, '2021-07-13 06:16:39', 1, 1);
INSERT INTO `realms_status` VALUES (381, 1, 'Dark', 1, 1, '2021-07-13 06:16:54', 1, 1);
INSERT INTO `realms_status` VALUES (382, 1, 'Dark', 1, 1, '2021-07-13 06:17:08', 1, 1);
INSERT INTO `realms_status` VALUES (383, 1, 'Dark', 1, 1, '2021-07-13 06:17:23', 1, 1);
INSERT INTO `realms_status` VALUES (384, 1, 'Dark', 1, 1, '2021-07-13 06:17:38', 1, 1);
INSERT INTO `realms_status` VALUES (385, 1, 'Dark', 1, 1, '2021-07-13 06:17:53', 1, 1);
INSERT INTO `realms_status` VALUES (386, 1, 'Dark', 1, 1, '2021-07-13 06:18:08', 1, 1);
INSERT INTO `realms_status` VALUES (387, 1, 'Dark', 1, 1, '2021-07-13 06:18:24', 1, 1);
INSERT INTO `realms_status` VALUES (388, 1, 'Dark', 1, 1, '2021-07-13 06:18:39', 1, 1);
INSERT INTO `realms_status` VALUES (389, 1, 'Dark', 1, 1, '2021-07-13 06:18:53', 1, 1);
INSERT INTO `realms_status` VALUES (390, 1, 'Dark', 1, 1, '2021-07-13 06:19:08', 1, 1);
INSERT INTO `realms_status` VALUES (391, 1, 'Dark', 1, 1, '2021-07-13 06:19:23', 1, 1);
INSERT INTO `realms_status` VALUES (392, 1, 'Dark', 1, 1, '2021-07-13 06:19:38', 1, 1);
INSERT INTO `realms_status` VALUES (393, 1, 'Dark', 1, 1, '2021-07-13 06:19:53', 1, 1);
INSERT INTO `realms_status` VALUES (394, 1, 'Dark', 1, 1, '2021-07-13 06:20:09', 1, 1);
INSERT INTO `realms_status` VALUES (395, 1, 'Dark', 1, 1, '2021-07-13 06:20:24', 1, 1);
INSERT INTO `realms_status` VALUES (396, 1, 'Dark', 1, 1, '2021-07-13 06:20:38', 1, 1);
INSERT INTO `realms_status` VALUES (397, 1, 'Dark', 1, 1, '2021-07-13 06:20:53', 1, 1);
INSERT INTO `realms_status` VALUES (398, 1, 'Dark', 1, 1, '2021-07-13 06:21:08', 1, 1);
INSERT INTO `realms_status` VALUES (399, 1, 'Dark', 1, 1, '2021-07-13 06:21:23', 1, 1);
INSERT INTO `realms_status` VALUES (400, 1, 'Dark', 1, 1, '2021-07-13 06:21:39', 1, 1);
INSERT INTO `realms_status` VALUES (401, 1, 'Dark', 1, 1, '2021-07-13 06:21:54', 1, 1);
INSERT INTO `realms_status` VALUES (402, 1, 'Dark', 1, 1, '2021-07-13 06:22:08', 1, 1);
INSERT INTO `realms_status` VALUES (403, 1, 'Dark', 1, 1, '2021-07-13 06:22:23', 1, 1);
INSERT INTO `realms_status` VALUES (404, 1, 'Dark', 1, 1, '2021-07-13 06:22:38', 1, 1);
INSERT INTO `realms_status` VALUES (405, 1, 'Dark', 1, 1, '2021-07-13 06:22:53', 1, 1);
INSERT INTO `realms_status` VALUES (406, 1, 'Dark', 1, 1, '2021-07-13 06:23:08', 1, 1);
INSERT INTO `realms_status` VALUES (407, 1, 'Dark', 1, 1, '2021-07-13 06:23:24', 1, 1);
INSERT INTO `realms_status` VALUES (408, 1, 'Dark', 1, 1, '2021-07-13 06:23:39', 1, 1);
INSERT INTO `realms_status` VALUES (409, 1, 'Dark', 1, 1, '2021-07-13 06:23:53', 1, 1);
INSERT INTO `realms_status` VALUES (410, 1, 'Dark', 1, 1, '2021-07-13 06:24:08', 1, 1);
INSERT INTO `realms_status` VALUES (411, 1, 'Dark', 1, 1, '2021-07-13 06:24:23', 1, 1);
INSERT INTO `realms_status` VALUES (412, 1, 'Dark', 1, 1, '2021-07-13 06:24:38', 1, 1);
INSERT INTO `realms_status` VALUES (413, 1, 'Dark', 1, 1, '2021-07-13 06:24:54', 1, 1);
INSERT INTO `realms_status` VALUES (414, 1, 'Dark', 1, 1, '2021-07-13 06:25:09', 1, 1);
INSERT INTO `realms_status` VALUES (415, 1, 'Dark', 1, 1, '2021-07-13 06:25:23', 1, 1);
INSERT INTO `realms_status` VALUES (416, 1, 'Dark', 1, 1, '2021-07-13 06:25:38', 1, 1);
INSERT INTO `realms_status` VALUES (417, 1, 'Dark', 1, 1, '2021-07-13 06:25:53', 1, 1);
INSERT INTO `realms_status` VALUES (418, 1, 'Dark', 1, 1, '2021-07-13 06:26:08', 1, 1);
INSERT INTO `realms_status` VALUES (419, 1, 'Dark', 1, 1, '2021-07-13 06:26:23', 1, 1);
INSERT INTO `realms_status` VALUES (420, 1, 'Dark', 1, 1, '2021-07-13 06:26:38', 1, 1);
INSERT INTO `realms_status` VALUES (421, 1, 'Dark', 1, 1, '2021-07-13 06:26:54', 1, 1);
INSERT INTO `realms_status` VALUES (422, 1, 'Dark', 1, 1, '2021-07-13 06:27:08', 1, 1);
INSERT INTO `realms_status` VALUES (423, 1, 'Dark', 1, 1, '2021-07-13 06:27:23', 1, 1);
INSERT INTO `realms_status` VALUES (424, 1, 'Dark', 1, 1, '2021-07-13 06:27:38', 1, 1);
INSERT INTO `realms_status` VALUES (425, 1, 'Dark', 1, 1, '2021-07-13 06:27:53', 1, 1);
INSERT INTO `realms_status` VALUES (426, 1, 'Dark', 1, 1, '2021-07-13 06:28:08', 1, 1);
INSERT INTO `realms_status` VALUES (427, 1, 'Dark', 1, 1, '2021-07-13 06:28:23', 1, 1);
INSERT INTO `realms_status` VALUES (428, 1, 'Dark', 1, 1, '2021-07-13 06:28:39', 1, 1);
INSERT INTO `realms_status` VALUES (429, 1, 'Dark', 1, 1, '2021-07-13 06:28:54', 1, 1);
INSERT INTO `realms_status` VALUES (430, 1, 'Dark', 1, 1, '2021-07-13 06:29:08', 1, 1);
INSERT INTO `realms_status` VALUES (431, 1, 'Dark', 1, 1, '2021-07-13 06:29:23', 1, 1);
INSERT INTO `realms_status` VALUES (432, 1, 'Dark', 1, 1, '2021-07-13 06:29:38', 1, 1);
INSERT INTO `realms_status` VALUES (433, 1, 'Dark', 1, 1, '2021-07-13 06:29:53', 1, 1);
INSERT INTO `realms_status` VALUES (434, 1, 'Dark', 1, 1, '2021-07-13 06:30:08', 1, 1);
INSERT INTO `realms_status` VALUES (435, 1, 'Dark', 1, 1, '2021-07-13 06:30:24', 1, 1);
INSERT INTO `realms_status` VALUES (436, 1, 'Dark', 1, 1, '2021-07-13 06:30:39', 1, 1);
INSERT INTO `realms_status` VALUES (437, 1, 'Dark', 1, 1, '2021-07-13 06:30:53', 1, 1);
INSERT INTO `realms_status` VALUES (438, 1, 'Dark', 1, 1, '2021-07-13 06:31:08', 1, 1);
INSERT INTO `realms_status` VALUES (439, 1, 'Dark', 1, 1, '2021-07-13 06:31:23', 1, 1);
INSERT INTO `realms_status` VALUES (440, 1, 'Dark', 1, 1, '2021-07-13 06:31:38', 1, 1);
INSERT INTO `realms_status` VALUES (441, 1, 'Dark', 1, 1, '2021-07-13 06:31:53', 1, 1);
INSERT INTO `realms_status` VALUES (442, 1, 'Dark', 1, 1, '2021-07-13 06:32:09', 1, 1);
INSERT INTO `realms_status` VALUES (443, 1, 'Dark', 1, 1, '2021-07-13 06:32:24', 1, 1);
INSERT INTO `realms_status` VALUES (444, 1, 'Dark', 1, 1, '2021-07-13 06:32:38', 1, 1);
INSERT INTO `realms_status` VALUES (445, 1, 'Dark', 1, 1, '2021-07-13 06:32:53', 1, 1);
INSERT INTO `realms_status` VALUES (446, 1, 'Dark', 1, 1, '2021-07-13 06:33:08', 1, 1);
INSERT INTO `realms_status` VALUES (447, 1, 'Dark', 1, 1, '2021-07-13 06:33:23', 1, 1);
INSERT INTO `realms_status` VALUES (448, 1, 'Dark', 1, 1, '2021-07-13 06:33:38', 1, 1);
INSERT INTO `realms_status` VALUES (449, 1, 'Dark', 1, 1, '2021-07-13 06:33:53', 1, 1);
INSERT INTO `realms_status` VALUES (450, 1, 'Dark', 1, 1, '2021-07-13 06:34:09', 1, 1);
INSERT INTO `realms_status` VALUES (451, 1, 'Dark', 1, 1, '2021-07-13 06:34:23', 1, 1);
INSERT INTO `realms_status` VALUES (452, 1, 'Dark', 1, 1, '2021-07-13 06:34:38', 1, 1);
INSERT INTO `realms_status` VALUES (453, 1, 'Dark', 1, 1, '2021-07-13 06:34:53', 1, 1);
INSERT INTO `realms_status` VALUES (454, 1, 'Dark', 1, 1, '2021-07-13 06:35:08', 1, 1);
INSERT INTO `realms_status` VALUES (455, 1, 'Dark', 1, 1, '2021-07-13 06:35:23', 1, 1);
INSERT INTO `realms_status` VALUES (456, 1, 'Dark', 1, 1, '2021-07-13 06:35:38', 1, 1);
INSERT INTO `realms_status` VALUES (457, 1, 'Dark', 1, 1, '2021-07-13 06:35:54', 1, 1);
INSERT INTO `realms_status` VALUES (458, 1, 'Dark', 1, 1, '2021-07-13 06:36:08', 1, 1);
INSERT INTO `realms_status` VALUES (459, 1, 'Dark', 1, 1, '2021-07-13 06:36:23', 1, 1);
INSERT INTO `realms_status` VALUES (460, 1, 'Dark', 1, 1, '2021-07-13 06:36:38', 1, 1);
INSERT INTO `realms_status` VALUES (461, 1, 'Dark', 1, 1, '2021-07-13 06:36:53', 1, 1);
INSERT INTO `realms_status` VALUES (462, 1, 'Dark', 1, 1, '2021-07-13 06:37:08', 1, 1);
INSERT INTO `realms_status` VALUES (463, 1, 'Dark', 1, 1, '2021-07-13 06:37:23', 1, 1);
INSERT INTO `realms_status` VALUES (464, 1, 'Dark', 1, 1, '2021-07-13 06:37:39', 1, 1);
INSERT INTO `realms_status` VALUES (465, 1, 'Dark', 1, 1, '2021-07-13 06:37:54', 1, 1);
INSERT INTO `realms_status` VALUES (466, 1, 'Dark', 1, 1, '2021-07-13 06:38:08', 1, 1);
INSERT INTO `realms_status` VALUES (467, 1, 'Dark', 1, 1, '2021-07-13 06:38:23', 1, 1);
INSERT INTO `realms_status` VALUES (468, 1, 'Dark', 1, 1, '2021-07-13 06:38:38', 1, 1);
INSERT INTO `realms_status` VALUES (469, 1, 'Dark', 1, 1, '2021-07-13 06:38:53', 1, 1);
INSERT INTO `realms_status` VALUES (470, 1, 'Dark', 1, 1, '2021-07-13 06:39:08', 1, 1);
INSERT INTO `realms_status` VALUES (471, 1, 'Dark', 1, 1, '2021-07-13 06:39:23', 1, 1);
INSERT INTO `realms_status` VALUES (472, 1, 'Dark', 1, 1, '2021-07-13 06:39:39', 1, 1);
INSERT INTO `realms_status` VALUES (473, 1, 'Dark', 1, 1, '2021-07-13 06:39:53', 1, 1);
INSERT INTO `realms_status` VALUES (474, 1, 'Dark', 1, 1, '2021-07-13 06:40:08', 1, 1);
INSERT INTO `realms_status` VALUES (475, 1, 'Dark', 1, 1, '2021-07-13 06:40:23', 1, 1);
INSERT INTO `realms_status` VALUES (476, 1, 'Dark', 1, 1, '2021-07-13 06:40:38', 1, 1);
INSERT INTO `realms_status` VALUES (477, 1, 'Dark', 1, 1, '2021-07-13 06:40:53', 1, 1);
INSERT INTO `realms_status` VALUES (478, 1, 'Dark', 1, 1, '2021-07-13 06:41:09', 1, 1);
INSERT INTO `realms_status` VALUES (479, 1, 'Dark', 1, 1, '2021-07-13 06:41:24', 1, 1);
INSERT INTO `realms_status` VALUES (480, 1, 'Dark', 1, 1, '2021-07-13 06:41:38', 1, 1);
INSERT INTO `realms_status` VALUES (481, 1, 'Dark', 1, 1, '2021-07-13 06:41:53', 1, 1);
INSERT INTO `realms_status` VALUES (482, 1, 'Dark', 1, 1, '2021-07-13 06:42:08', 1, 1);
INSERT INTO `realms_status` VALUES (483, 1, 'Dark', 1, 1, '2021-07-13 06:42:23', 1, 1);
INSERT INTO `realms_status` VALUES (484, 1, 'Dark', 1, 1, '2021-07-13 06:42:38', 1, 1);
INSERT INTO `realms_status` VALUES (485, 1, 'Dark', 1, 1, '2021-07-13 06:42:54', 1, 1);
INSERT INTO `realms_status` VALUES (486, 1, 'Dark', 1, 1, '2021-07-13 06:43:09', 1, 1);
INSERT INTO `realms_status` VALUES (487, 1, 'Dark', 1, 1, '2021-07-13 06:43:23', 1, 1);
INSERT INTO `realms_status` VALUES (488, 1, 'Dark', 1, 1, '2021-07-13 06:43:38', 1, 1);
INSERT INTO `realms_status` VALUES (489, 1, 'Dark', 1, 1, '2021-07-13 06:43:53', 1, 1);
INSERT INTO `realms_status` VALUES (490, 1, 'Dark', 1, 1, '2021-07-13 06:44:08', 1, 1);
INSERT INTO `realms_status` VALUES (491, 1, 'Dark', 1, 1, '2021-07-13 06:44:23', 1, 1);
INSERT INTO `realms_status` VALUES (492, 1, 'Dark', 1, 1, '2021-07-13 06:44:38', 1, 1);
INSERT INTO `realms_status` VALUES (493, 1, 'Dark', 1, 1, '2021-07-13 06:44:54', 1, 1);
INSERT INTO `realms_status` VALUES (494, 1, 'Dark', 1, 1, '2021-07-13 06:45:09', 1, 1);
INSERT INTO `realms_status` VALUES (495, 1, 'Dark', 1, 1, '2021-07-13 06:45:23', 1, 1);
INSERT INTO `realms_status` VALUES (496, 1, 'Dark', 1, 1, '2021-07-13 06:45:38', 1, 1);
INSERT INTO `realms_status` VALUES (497, 1, 'Dark', 1, 1, '2021-07-13 06:45:53', 1, 1);
INSERT INTO `realms_status` VALUES (498, 1, 'Dark', 1, 1, '2021-07-13 06:46:08', 1, 1);
INSERT INTO `realms_status` VALUES (499, 1, 'Dark', 1, 1, '2021-07-13 06:46:23', 1, 1);
INSERT INTO `realms_status` VALUES (500, 1, 'Dark', 1, 1, '2021-07-13 06:46:39', 1, 1);
INSERT INTO `realms_status` VALUES (501, 1, 'Dark', 1, 1, '2021-07-13 06:46:54', 1, 1);
INSERT INTO `realms_status` VALUES (502, 1, 'Dark', 1, 1, '2021-07-13 06:47:08', 1, 1);
INSERT INTO `realms_status` VALUES (503, 1, 'Dark', 1, 1, '2021-07-13 06:47:23', 1, 1);
INSERT INTO `realms_status` VALUES (504, 1, 'Dark', 1, 1, '2021-07-13 06:47:38', 1, 1);
INSERT INTO `realms_status` VALUES (505, 1, 'Dark', 1, 1, '2021-07-13 06:47:53', 1, 1);
INSERT INTO `realms_status` VALUES (506, 1, 'Dark', 1, 1, '2021-07-13 06:48:08', 1, 1);
INSERT INTO `realms_status` VALUES (507, 1, 'Dark', 1, 1, '2021-07-13 06:48:24', 1, 1);
INSERT INTO `realms_status` VALUES (508, 1, 'Dark', 1, 1, '2021-07-13 06:48:39', 1, 1);
INSERT INTO `realms_status` VALUES (509, 1, 'Dark', 1, 1, '2021-07-13 06:48:54', 1, 1);
INSERT INTO `realms_status` VALUES (510, 1, 'Dark', 1, 1, '2021-07-13 06:49:09', 1, 1);
INSERT INTO `realms_status` VALUES (511, 1, 'Dark', 1, 1, '2021-07-13 06:49:24', 1, 1);
INSERT INTO `realms_status` VALUES (512, 1, 'Dark', 1, 1, '2021-07-13 06:49:39', 1, 1);
INSERT INTO `realms_status` VALUES (513, 1, 'Dark', 1, 1, '2021-07-13 06:49:54', 1, 1);
INSERT INTO `realms_status` VALUES (514, 1, 'Dark', 1, 1, '2021-07-13 06:50:10', 1, 1);
INSERT INTO `realms_status` VALUES (515, 1, 'Dark', 1, 1, '2021-07-13 06:50:25', 1, 1);
INSERT INTO `realms_status` VALUES (516, 1, 'Dark', 1, 1, '2021-07-13 06:50:40', 1, 1);
INSERT INTO `realms_status` VALUES (517, 1, 'Dark', 1, 1, '2021-07-13 06:50:55', 1, 1);
INSERT INTO `realms_status` VALUES (518, 1, 'Dark', 1, 1, '2021-07-13 06:51:10', 1, 1);
INSERT INTO `realms_status` VALUES (519, 1, 'Dark', 1, 1, '2021-07-13 06:51:25', 1, 1);
INSERT INTO `realms_status` VALUES (520, 1, 'Dark', 1, 1, '2021-07-13 06:51:41', 1, 1);
INSERT INTO `realms_status` VALUES (521, 1, 'Dark', 1, 1, '2021-07-13 06:51:56', 1, 1);
INSERT INTO `realms_status` VALUES (522, 1, 'Dark', 1, 1, '2021-07-13 06:52:10', 1, 1);
INSERT INTO `realms_status` VALUES (523, 1, 'Dark', 1, 1, '2021-07-13 06:52:25', 1, 1);
INSERT INTO `realms_status` VALUES (524, 1, 'Dark', 1, 1, '2021-07-13 06:52:40', 1, 1);
INSERT INTO `realms_status` VALUES (525, 1, 'Dark', 1, 1, '2021-07-13 06:52:55', 1, 1);
INSERT INTO `realms_status` VALUES (526, 1, 'Dark', 1, 1, '2021-07-13 06:53:10', 1, 1);
INSERT INTO `realms_status` VALUES (527, 1, 'Dark', 1, 1, '2021-07-13 06:53:25', 1, 1);
INSERT INTO `realms_status` VALUES (528, 1, 'Dark', 1, 1, '2021-07-13 06:53:41', 1, 1);
INSERT INTO `realms_status` VALUES (529, 1, 'Dark', 1, 1, '2021-07-13 06:53:56', 1, 1);
INSERT INTO `realms_status` VALUES (530, 1, 'Dark', 1, 1, '2021-07-13 06:54:10', 1, 1);
INSERT INTO `realms_status` VALUES (531, 1, 'Dark', 1, 1, '2021-07-13 06:54:25', 1, 1);
INSERT INTO `realms_status` VALUES (532, 1, 'Dark', 1, 1, '2021-07-13 06:54:40', 1, 1);
INSERT INTO `realms_status` VALUES (533, 1, 'Dark', 1, 1, '2021-07-13 06:54:55', 1, 1);
INSERT INTO `realms_status` VALUES (534, 1, 'Dark', 1, 1, '2021-07-13 06:55:10', 1, 1);
INSERT INTO `realms_status` VALUES (535, 1, 'Dark', 1, 1, '2021-07-13 06:55:26', 1, 1);
INSERT INTO `realms_status` VALUES (536, 1, 'Dark', 1, 1, '2021-07-13 06:55:41', 1, 1);
INSERT INTO `realms_status` VALUES (537, 1, 'Dark', 1, 1, '2021-07-13 06:55:55', 1, 1);
INSERT INTO `realms_status` VALUES (538, 1, 'Dark', 1, 1, '2021-07-13 06:56:10', 1, 1);
INSERT INTO `realms_status` VALUES (539, 1, 'Dark', 1, 1, '2021-07-13 06:56:25', 1, 1);
INSERT INTO `realms_status` VALUES (540, 1, 'Dark', 1, 1, '2021-07-13 06:56:40', 1, 1);
INSERT INTO `realms_status` VALUES (541, 1, 'Dark', 1, 1, '2021-07-13 06:56:55', 1, 1);
INSERT INTO `realms_status` VALUES (542, 1, 'Dark', 1, 1, '2021-07-13 06:57:11', 1, 1);
INSERT INTO `realms_status` VALUES (543, 1, 'Dark', 1, 1, '2021-07-13 06:57:26', 1, 1);
INSERT INTO `realms_status` VALUES (544, 1, 'Dark', 1, 1, '2021-07-13 06:57:40', 1, 1);
INSERT INTO `realms_status` VALUES (545, 1, 'Dark', 1, 1, '2021-07-13 06:57:55', 1, 1);
INSERT INTO `realms_status` VALUES (546, 1, 'Dark', 1, 1, '2021-07-13 06:58:10', 1, 1);
INSERT INTO `realms_status` VALUES (547, 1, 'Dark', 1, 1, '2021-07-13 06:58:25', 1, 1);
INSERT INTO `realms_status` VALUES (548, 1, 'Dark', 1, 1, '2021-07-13 06:58:40', 1, 1);
INSERT INTO `realms_status` VALUES (549, 1, 'Dark', 1, 1, '2021-07-13 06:58:55', 1, 1);
INSERT INTO `realms_status` VALUES (550, 1, 'Dark', 1, 1, '2021-07-13 06:59:11', 1, 1);
INSERT INTO `realms_status` VALUES (551, 1, 'Dark', 1, 1, '2021-07-13 06:59:26', 1, 1);
INSERT INTO `realms_status` VALUES (552, 1, 'Dark', 1, 1, '2021-07-13 06:59:40', 1, 1);
INSERT INTO `realms_status` VALUES (553, 1, 'Dark', 1, 1, '2021-07-13 06:59:55', 1, 1);
INSERT INTO `realms_status` VALUES (554, 1, 'Dark', 1, 1, '2021-07-13 07:00:10', 1, 1);
INSERT INTO `realms_status` VALUES (555, 1, 'Dark', 1, 1, '2021-07-13 07:00:25', 1, 1);
INSERT INTO `realms_status` VALUES (556, 1, 'Dark', 1, 1, '2021-07-13 07:00:40', 1, 1);
INSERT INTO `realms_status` VALUES (557, 1, 'Dark', 1, 1, '2021-07-13 07:00:56', 1, 1);
INSERT INTO `realms_status` VALUES (558, 1, 'Dark', 1, 1, '2021-07-13 07:01:10', 1, 1);
INSERT INTO `realms_status` VALUES (559, 1, 'Dark', 1, 1, '2021-07-13 07:01:25', 1, 1);
INSERT INTO `realms_status` VALUES (560, 1, 'Dark', 1, 1, '2021-07-13 07:01:40', 1, 1);
INSERT INTO `realms_status` VALUES (561, 1, 'Dark', 1, 1, '2021-07-13 07:01:55', 1, 1);
INSERT INTO `realms_status` VALUES (562, 1, 'Dark', 1, 1, '2021-07-13 07:02:10', 1, 1);
INSERT INTO `realms_status` VALUES (563, 1, 'Dark', 1, 1, '2021-07-13 07:02:26', 1, 1);
INSERT INTO `realms_status` VALUES (564, 1, 'Dark', 1, 1, '2021-07-13 07:02:41', 1, 1);
INSERT INTO `realms_status` VALUES (565, 1, 'Dark', 1, 1, '2021-07-13 07:02:55', 1, 1);
INSERT INTO `realms_status` VALUES (566, 1, 'Dark', 1, 1, '2021-07-13 07:03:10', 1, 1);
INSERT INTO `realms_status` VALUES (567, 1, 'Dark', 1, 1, '2021-07-13 07:03:25', 1, 1);
INSERT INTO `realms_status` VALUES (568, 1, 'Dark', 1, 1, '2021-07-13 07:03:40', 1, 1);
INSERT INTO `realms_status` VALUES (569, 1, 'Dark', 1, 1, '2021-07-13 07:03:55', 1, 1);
INSERT INTO `realms_status` VALUES (570, 1, 'Dark', 1, 1, '2021-07-13 07:04:10', 1, 1);
INSERT INTO `realms_status` VALUES (571, 1, 'Dark', 1, 1, '2021-07-13 07:04:26', 1, 1);
INSERT INTO `realms_status` VALUES (572, 1, 'Dark', 1, 1, '2021-07-13 07:04:41', 1, 1);
INSERT INTO `realms_status` VALUES (573, 1, 'Dark', 1, 1, '2021-07-13 07:04:56', 1, 1);
INSERT INTO `realms_status` VALUES (574, 1, 'Dark', 1, 1, '2021-07-13 07:05:11', 1, 1);
INSERT INTO `realms_status` VALUES (575, 1, 'Dark', 1, 1, '2021-07-13 07:05:26', 1, 1);
INSERT INTO `realms_status` VALUES (576, 1, 'Dark', 1, 1, '2021-07-13 07:05:41', 1, 1);
INSERT INTO `realms_status` VALUES (577, 1, 'Dark', 1, 1, '2021-07-13 07:05:56', 1, 1);
INSERT INTO `realms_status` VALUES (578, 1, 'Dark', 1, 1, '2021-07-13 07:06:12', 1, 1);
INSERT INTO `realms_status` VALUES (579, 1, 'Dark', 1, 1, '2021-07-13 07:06:27', 1, 1);
INSERT INTO `realms_status` VALUES (580, 1, 'Dark', 1, 1, '2021-07-13 07:06:42', 1, 1);
INSERT INTO `realms_status` VALUES (581, 1, 'Dark', 1, 1, '2021-07-13 07:06:57', 1, 1);
INSERT INTO `realms_status` VALUES (582, 1, 'Dark', 1, 1, '2021-07-13 07:07:12', 1, 1);
INSERT INTO `realms_status` VALUES (583, 1, 'Dark', 1, 1, '2021-07-13 07:07:27', 1, 1);
INSERT INTO `realms_status` VALUES (584, 1, 'Dark', 1, 1, '2021-07-13 07:07:42', 1, 1);
INSERT INTO `realms_status` VALUES (585, 1, 'Dark', 1, 1, '2021-07-13 07:07:58', 1, 1);
INSERT INTO `realms_status` VALUES (586, 1, 'Dark', 1, 1, '2021-07-13 07:08:12', 1, 1);
INSERT INTO `realms_status` VALUES (587, 1, 'Dark', 1, 1, '2021-07-13 07:08:27', 1, 1);
INSERT INTO `realms_status` VALUES (588, 1, 'Dark', 1, 1, '2021-07-13 07:08:42', 1, 1);
INSERT INTO `realms_status` VALUES (589, 1, 'Dark', 1, 1, '2021-07-13 07:08:57', 1, 1);
INSERT INTO `realms_status` VALUES (590, 1, 'Dark', 1, 1, '2021-07-13 07:09:12', 1, 1);
INSERT INTO `realms_status` VALUES (591, 1, 'Dark', 1, 1, '2021-07-13 07:09:27', 1, 1);
INSERT INTO `realms_status` VALUES (592, 1, 'Dark', 1, 1, '2021-07-13 07:09:43', 1, 1);
INSERT INTO `realms_status` VALUES (593, 1, 'Dark', 1, 1, '2021-07-13 07:09:57', 1, 1);
INSERT INTO `realms_status` VALUES (594, 1, 'Dark', 1, 1, '2021-07-13 07:10:12', 1, 1);
INSERT INTO `realms_status` VALUES (595, 1, 'Dark', 1, 1, '2021-07-13 07:10:27', 1, 1);
INSERT INTO `realms_status` VALUES (596, 1, 'Dark', 1, 1, '2021-07-13 07:10:42', 1, 1);
INSERT INTO `realms_status` VALUES (597, 1, 'Dark', 1, 1, '2021-07-13 07:10:57', 1, 1);
INSERT INTO `realms_status` VALUES (598, 1, 'Dark', 1, 1, '2021-07-13 07:11:12', 1, 1);
INSERT INTO `realms_status` VALUES (599, 1, 'Dark', 1, 1, '2021-07-13 07:11:28', 1, 1);
INSERT INTO `realms_status` VALUES (600, 1, 'Dark', 1, 1, '2021-07-13 07:11:43', 1, 1);
INSERT INTO `realms_status` VALUES (601, 1, 'Dark', 1, 1, '2021-07-13 07:11:57', 1, 1);
INSERT INTO `realms_status` VALUES (602, 1, 'Dark', 1, 1, '2021-07-13 07:12:12', 1, 1);
INSERT INTO `realms_status` VALUES (603, 1, 'Dark', 1, 1, '2021-07-13 07:12:27', 1, 1);
INSERT INTO `realms_status` VALUES (604, 1, 'Dark', 1, 1, '2021-07-13 07:12:42', 1, 1);
INSERT INTO `realms_status` VALUES (605, 1, 'Dark', 1, 1, '2021-07-13 07:12:57', 1, 1);
INSERT INTO `realms_status` VALUES (606, 1, 'Dark', 1, 1, '2021-07-13 07:13:13', 1, 1);
INSERT INTO `realms_status` VALUES (607, 1, 'Dark', 1, 1, '2021-07-13 07:13:28', 1, 1);
INSERT INTO `realms_status` VALUES (608, 1, 'Dark', 1, 1, '2021-07-13 07:13:42', 1, 1);
INSERT INTO `realms_status` VALUES (609, 1, 'Dark', 1, 1, '2021-07-13 07:13:57', 1, 1);
INSERT INTO `realms_status` VALUES (610, 1, 'Dark', 1, 1, '2021-07-13 07:14:12', 1, 1);
INSERT INTO `realms_status` VALUES (611, 1, 'Dark', 1, 1, '2021-07-13 07:14:27', 1, 1);
INSERT INTO `realms_status` VALUES (612, 1, 'Dark', 1, 1, '2021-07-13 07:14:42', 1, 1);
INSERT INTO `realms_status` VALUES (613, 1, 'Dark', 1, 1, '2021-07-13 07:14:58', 1, 1);
INSERT INTO `realms_status` VALUES (614, 1, 'Dark', 1, 1, '2021-07-13 07:15:12', 1, 1);
INSERT INTO `realms_status` VALUES (615, 1, 'Dark', 1, 1, '2021-07-13 07:15:27', 1, 1);
INSERT INTO `realms_status` VALUES (616, 1, 'Dark', 1, 1, '2021-07-13 07:15:42', 1, 1);
INSERT INTO `realms_status` VALUES (617, 1, 'Dark', 1, 1, '2021-07-13 07:15:57', 1, 1);
INSERT INTO `realms_status` VALUES (618, 1, 'Dark', 1, 1, '2021-07-13 07:16:12', 1, 1);
INSERT INTO `realms_status` VALUES (619, 1, 'Dark', 1, 1, '2021-07-13 07:16:28', 1, 1);
INSERT INTO `realms_status` VALUES (620, 1, 'Dark', 1, 1, '2021-07-13 07:16:43', 1, 1);
INSERT INTO `realms_status` VALUES (621, 1, 'Dark', 1, 1, '2021-07-13 07:16:57', 1, 1);
INSERT INTO `realms_status` VALUES (622, 1, 'Dark', 1, 1, '2021-07-13 07:17:12', 1, 1);
INSERT INTO `realms_status` VALUES (623, 1, 'Dark', 1, 1, '2021-07-13 07:17:27', 1, 1);
INSERT INTO `realms_status` VALUES (624, 1, 'Dark', 1, 1, '2021-07-13 07:17:42', 1, 1);
INSERT INTO `realms_status` VALUES (625, 1, 'Dark', 1, 1, '2021-07-13 07:17:57', 1, 1);
INSERT INTO `realms_status` VALUES (626, 1, 'Dark', 1, 1, '2021-07-13 07:18:13', 1, 1);
INSERT INTO `realms_status` VALUES (627, 1, 'Dark', 1, 1, '2021-07-13 07:18:28', 1, 1);
INSERT INTO `realms_status` VALUES (628, 1, 'Dark', 1, 1, '2021-07-13 07:18:42', 1, 1);
INSERT INTO `realms_status` VALUES (629, 1, 'Dark', 1, 1, '2021-07-13 07:18:57', 1, 1);
INSERT INTO `realms_status` VALUES (630, 1, 'Dark', 1, 1, '2021-07-13 07:19:12', 1, 1);
INSERT INTO `realms_status` VALUES (631, 1, 'Dark', 1, 1, '2021-07-13 07:19:27', 1, 1);
INSERT INTO `realms_status` VALUES (632, 1, 'Dark', 1, 1, '2021-07-13 07:19:42', 1, 1);
INSERT INTO `realms_status` VALUES (633, 1, 'Dark', 1, 1, '2021-07-13 07:19:58', 1, 1);
INSERT INTO `realms_status` VALUES (634, 1, 'Dark', 1, 1, '2021-07-13 07:20:12', 1, 1);
INSERT INTO `realms_status` VALUES (635, 1, 'Dark', 1, 1, '2021-07-13 07:20:27', 1, 1);
INSERT INTO `realms_status` VALUES (636, 1, 'Dark', 1, 1, '2021-07-13 07:20:42', 1, 1);
INSERT INTO `realms_status` VALUES (637, 1, 'Dark', 1, 1, '2021-07-13 07:20:57', 1, 1);
INSERT INTO `realms_status` VALUES (638, 1, 'Dark', 1, 1, '2021-07-13 07:21:12', 1, 1);
INSERT INTO `realms_status` VALUES (639, 1, 'Dark', 1, 1, '2021-07-13 07:21:27', 1, 1);
INSERT INTO `realms_status` VALUES (640, 1, 'Dark', 1, 1, '2021-07-13 07:21:43', 1, 1);
INSERT INTO `realms_status` VALUES (641, 1, 'Dark', 1, 1, '2021-07-13 07:21:58', 1, 1);
INSERT INTO `realms_status` VALUES (642, 1, 'Dark', 1, 1, '2021-07-13 07:22:12', 1, 1);
INSERT INTO `realms_status` VALUES (643, 1, 'Dark', 1, 1, '2021-07-13 07:22:27', 1, 1);
INSERT INTO `realms_status` VALUES (644, 1, 'Dark', 1, 1, '2021-07-13 07:22:42', 1, 1);
INSERT INTO `realms_status` VALUES (645, 1, 'Dark', 1, 1, '2021-07-13 07:22:57', 1, 1);
INSERT INTO `realms_status` VALUES (646, 1, 'Dark', 1, 1, '2021-07-13 07:23:12', 1, 1);
INSERT INTO `realms_status` VALUES (647, 1, 'Dark', 1, 1, '2021-07-13 07:23:28', 1, 1);
INSERT INTO `realms_status` VALUES (648, 1, 'Dark', 1, 1, '2021-07-13 07:23:43', 1, 1);
INSERT INTO `realms_status` VALUES (649, 1, 'Dark', 1, 1, '2021-07-13 07:23:57', 1, 1);
INSERT INTO `realms_status` VALUES (650, 1, 'Dark', 1, 1, '2021-07-13 07:24:12', 1, 1);
INSERT INTO `realms_status` VALUES (651, 1, 'Dark', 1, 1, '2021-07-13 07:24:27', 1, 1);
INSERT INTO `realms_status` VALUES (652, 1, 'Dark', 1, 1, '2021-07-13 07:24:42', 1, 1);
INSERT INTO `realms_status` VALUES (653, 1, 'Dark', 1, 1, '2021-07-13 07:24:57', 1, 1);
INSERT INTO `realms_status` VALUES (654, 1, 'Dark', 1, 1, '2021-07-13 07:25:12', 1, 1);
INSERT INTO `realms_status` VALUES (655, 1, 'Dark', 1, 1, '2021-07-13 07:25:28', 1, 1);
INSERT INTO `realms_status` VALUES (656, 1, 'Dark', 1, 1, '2021-07-13 07:25:42', 1, 1);
INSERT INTO `realms_status` VALUES (657, 1, 'Dark', 1, 1, '2021-07-13 07:25:57', 1, 1);
INSERT INTO `realms_status` VALUES (658, 1, 'Dark', 1, 1, '2021-07-13 07:26:12', 1, 1);
INSERT INTO `realms_status` VALUES (659, 1, 'Dark', 1, 1, '2021-07-13 07:26:27', 1, 1);
INSERT INTO `realms_status` VALUES (660, 1, 'Dark', 1, 1, '2021-07-13 07:26:42', 1, 1);
INSERT INTO `realms_status` VALUES (661, 1, 'Dark', 1, 1, '2021-07-13 07:26:58', 1, 1);
INSERT INTO `realms_status` VALUES (662, 1, 'Dark', 1, 1, '2021-07-13 07:27:13', 1, 1);
INSERT INTO `realms_status` VALUES (663, 1, 'Dark', 1, 1, '2021-07-13 07:27:27', 1, 1);
INSERT INTO `realms_status` VALUES (664, 1, 'Dark', 1, 1, '2021-07-13 07:27:42', 1, 1);
INSERT INTO `realms_status` VALUES (665, 1, 'Dark', 1, 1, '2021-07-13 07:27:57', 1, 1);
INSERT INTO `realms_status` VALUES (666, 1, 'Dark', 1, 1, '2021-07-13 07:28:12', 1, 1);
INSERT INTO `realms_status` VALUES (667, 1, 'Dark', 1, 1, '2021-07-13 07:28:27', 1, 1);
INSERT INTO `realms_status` VALUES (668, 1, 'Dark', 1, 1, '2021-07-13 07:28:43', 1, 1);
INSERT INTO `realms_status` VALUES (669, 1, 'Dark', 1, 1, '2021-07-13 07:28:57', 1, 1);
INSERT INTO `realms_status` VALUES (670, 1, 'Dark', 1, 1, '2021-07-13 07:29:12', 1, 1);
INSERT INTO `realms_status` VALUES (671, 1, 'Dark', 1, 1, '2021-07-13 07:29:27', 1, 1);
INSERT INTO `realms_status` VALUES (672, 1, 'Dark', 1, 1, '2021-07-13 07:29:42', 1, 1);
INSERT INTO `realms_status` VALUES (673, 1, 'Dark', 1, 1, '2021-07-13 07:29:57', 1, 1);
INSERT INTO `realms_status` VALUES (674, 1, 'Dark', 1, 1, '2021-07-13 07:30:12', 1, 1);
INSERT INTO `realms_status` VALUES (675, 1, 'Dark', 1, 1, '2021-07-13 07:30:28', 1, 1);
INSERT INTO `realms_status` VALUES (676, 1, 'Dark', 1, 1, '2021-07-13 07:30:43', 1, 1);
INSERT INTO `realms_status` VALUES (677, 1, 'Dark', 1, 1, '2021-07-13 07:30:58', 1, 1);
INSERT INTO `realms_status` VALUES (678, 1, 'Dark', 1, 1, '2021-07-13 07:31:13', 1, 1);
INSERT INTO `realms_status` VALUES (679, 1, 'Dark', 1, 1, '2021-07-13 07:31:28', 1, 1);
INSERT INTO `realms_status` VALUES (680, 1, 'Dark', 1, 1, '2021-07-13 07:31:43', 1, 1);
INSERT INTO `realms_status` VALUES (681, 1, 'Dark', 1, 1, '2021-07-13 07:31:58', 1, 1);
INSERT INTO `realms_status` VALUES (682, 1, 'Dark', 1, 1, '2021-07-13 07:32:14', 1, 1);
INSERT INTO `realms_status` VALUES (683, 1, 'Dark', 1, 1, '2021-07-13 07:32:29', 1, 1);
INSERT INTO `realms_status` VALUES (684, 1, 'Dark', 1, 1, '2021-07-13 07:32:43', 1, 1);
INSERT INTO `realms_status` VALUES (685, 1, 'Dark', 1, 1, '2021-07-13 07:32:58', 1, 1);
INSERT INTO `realms_status` VALUES (686, 1, 'Dark', 1, 1, '2021-07-13 07:33:13', 1, 1);
INSERT INTO `realms_status` VALUES (687, 1, 'Dark', 1, 1, '2021-07-13 07:33:28', 1, 1);
INSERT INTO `realms_status` VALUES (688, 1, 'Dark', 1, 1, '2021-07-13 07:33:43', 1, 1);
INSERT INTO `realms_status` VALUES (689, 1, 'Dark', 1, 1, '2021-07-13 07:33:59', 1, 1);
INSERT INTO `realms_status` VALUES (690, 1, 'Dark', 1, 1, '2021-07-13 07:34:14', 1, 1);
INSERT INTO `realms_status` VALUES (691, 1, 'Dark', 1, 1, '2021-07-13 07:34:28', 1, 1);
INSERT INTO `realms_status` VALUES (692, 1, 'Dark', 1, 1, '2021-07-13 07:34:43', 1, 1);
INSERT INTO `realms_status` VALUES (693, 1, 'Dark', 1, 1, '2021-07-13 07:34:58', 1, 1);
INSERT INTO `realms_status` VALUES (694, 1, 'Dark', 1, 1, '2021-07-13 07:35:13', 1, 1);
INSERT INTO `realms_status` VALUES (695, 1, 'Dark', 1, 1, '2021-07-13 07:35:28', 1, 1);
INSERT INTO `realms_status` VALUES (696, 1, 'Dark', 1, 1, '2021-07-13 07:35:44', 1, 1);
INSERT INTO `realms_status` VALUES (697, 1, 'Dark', 1, 1, '2021-07-13 07:35:59', 1, 1);
INSERT INTO `realms_status` VALUES (698, 1, 'Dark', 1, 1, '2021-07-13 07:36:13', 1, 1);
INSERT INTO `realms_status` VALUES (699, 1, 'Dark', 1, 1, '2021-07-13 07:36:28', 1, 1);
INSERT INTO `realms_status` VALUES (700, 1, 'Dark', 1, 1, '2021-07-13 07:36:43', 1, 1);
INSERT INTO `realms_status` VALUES (701, 1, 'Dark', 1, 1, '2021-07-13 07:36:58', 1, 1);
INSERT INTO `realms_status` VALUES (702, 1, 'Dark', 1, 1, '2021-07-13 07:37:13', 1, 1);
INSERT INTO `realms_status` VALUES (703, 1, 'Dark', 1, 1, '2021-07-13 07:37:29', 1, 1);
INSERT INTO `realms_status` VALUES (704, 1, 'Dark', 1, 1, '2021-07-13 07:37:44', 1, 1);
INSERT INTO `realms_status` VALUES (705, 1, 'Dark', 1, 1, '2021-07-13 07:37:58', 1, 1);
INSERT INTO `realms_status` VALUES (706, 1, 'Dark', 1, 1, '2021-07-13 07:38:13', 1, 1);
INSERT INTO `realms_status` VALUES (707, 1, 'Dark', 1, 1, '2021-07-13 07:38:28', 1, 1);
INSERT INTO `realms_status` VALUES (708, 1, 'Dark', 1, 1, '2021-07-13 07:38:43', 1, 1);
INSERT INTO `realms_status` VALUES (709, 1, 'Dark', 1, 1, '2021-07-13 07:38:58', 1, 1);
INSERT INTO `realms_status` VALUES (710, 1, 'Dark', 1, 1, '2021-07-13 07:39:13', 1, 1);
INSERT INTO `realms_status` VALUES (711, 1, 'Dark', 1, 1, '2021-07-13 07:39:29', 1, 1);
INSERT INTO `realms_status` VALUES (712, 1, 'Dark', 1, 1, '2021-07-13 07:39:43', 1, 1);
INSERT INTO `realms_status` VALUES (713, 1, 'Dark', 1, 1, '2021-07-13 07:39:58', 1, 1);
INSERT INTO `realms_status` VALUES (714, 1, 'Dark', 1, 1, '2021-07-13 07:40:13', 1, 1);
INSERT INTO `realms_status` VALUES (715, 1, 'Dark', 1, 1, '2021-07-13 07:40:28', 1, 1);
INSERT INTO `realms_status` VALUES (716, 1, 'Dark', 1, 1, '2021-07-13 07:40:43', 1, 1);
INSERT INTO `realms_status` VALUES (717, 1, 'Dark', 1, 1, '2021-07-13 07:40:58', 1, 1);
INSERT INTO `realms_status` VALUES (718, 1, 'Dark', 1, 1, '2021-07-13 07:41:14', 1, 1);
INSERT INTO `realms_status` VALUES (719, 1, 'Dark', 1, 1, '2021-07-13 07:41:29', 1, 1);
INSERT INTO `realms_status` VALUES (720, 1, 'Dark', 1, 1, '2021-07-13 07:41:43', 1, 1);
INSERT INTO `realms_status` VALUES (721, 1, 'Dark', 1, 1, '2021-07-13 07:41:58', 1, 1);
INSERT INTO `realms_status` VALUES (722, 1, 'Dark', 1, 1, '2021-07-13 07:42:13', 1, 1);
INSERT INTO `realms_status` VALUES (723, 1, 'Dark', 1, 1, '2021-07-13 07:42:28', 1, 1);
INSERT INTO `realms_status` VALUES (724, 1, 'Dark', 1, 1, '2021-07-13 07:42:43', 1, 1);
INSERT INTO `realms_status` VALUES (725, 1, 'Dark', 1, 1, '2021-07-13 07:42:59', 1, 1);
INSERT INTO `realms_status` VALUES (726, 1, 'Dark', 1, 1, '2021-07-13 07:43:14', 1, 1);
INSERT INTO `realms_status` VALUES (727, 1, 'Dark', 1, 1, '2021-07-13 07:43:29', 1, 1);
INSERT INTO `realms_status` VALUES (728, 1, 'Dark', 1, 1, '2021-07-13 07:43:43', 1, 1);
INSERT INTO `realms_status` VALUES (729, 1, 'Dark', 1, 1, '2021-07-13 07:43:58', 1, 1);
INSERT INTO `realms_status` VALUES (730, 1, 'Dark', 1, 1, '2021-07-13 07:44:13', 1, 1);
INSERT INTO `realms_status` VALUES (731, 1, 'Dark', 1, 1, '2021-07-13 07:44:28', 1, 1);
INSERT INTO `realms_status` VALUES (732, 1, 'Dark', 1, 1, '2021-07-13 07:44:43', 1, 1);
INSERT INTO `realms_status` VALUES (733, 1, 'Dark', 1, 1, '2021-07-13 07:44:59', 1, 1);
INSERT INTO `realms_status` VALUES (734, 1, 'Dark', 1, 1, '2021-07-13 07:45:14', 1, 1);
INSERT INTO `realms_status` VALUES (735, 1, 'Dark', 1, 1, '2021-07-13 07:45:28', 1, 1);
INSERT INTO `realms_status` VALUES (736, 1, 'Dark', 1, 1, '2021-07-13 07:45:43', 1, 1);
INSERT INTO `realms_status` VALUES (737, 1, 'Dark', 1, 1, '2021-07-13 07:45:58', 1, 1);
INSERT INTO `realms_status` VALUES (738, 1, 'Dark', 1, 1, '2021-07-13 07:46:13', 1, 1);
INSERT INTO `realms_status` VALUES (739, 1, 'Dark', 1, 1, '2021-07-13 07:46:28', 1, 1);
INSERT INTO `realms_status` VALUES (740, 1, 'Dark', 1, 1, '2021-07-13 07:46:44', 1, 1);
INSERT INTO `realms_status` VALUES (741, 1, 'Dark', 1, 1, '2021-07-13 07:46:59', 1, 1);
INSERT INTO `realms_status` VALUES (742, 1, 'Dark', 1, 1, '2021-07-13 07:47:13', 1, 1);
INSERT INTO `realms_status` VALUES (743, 1, 'Dark', 1, 1, '2021-07-13 07:47:28', 1, 1);
INSERT INTO `realms_status` VALUES (744, 1, 'Dark', 1, 1, '2021-07-13 07:47:43', 1, 1);
INSERT INTO `realms_status` VALUES (745, 1, 'Dark', 1, 1, '2021-07-13 07:47:58', 1, 1);
INSERT INTO `realms_status` VALUES (746, 1, 'Dark', 1, 1, '2021-07-13 07:48:13', 1, 1);
INSERT INTO `realms_status` VALUES (747, 1, 'Dark', 1, 1, '2021-07-13 07:48:29', 1, 1);
INSERT INTO `realms_status` VALUES (748, 1, 'Dark', 1, 1, '2021-07-13 07:48:43', 1, 1);
INSERT INTO `realms_status` VALUES (749, 1, 'Dark', 1, 1, '2021-07-13 07:48:58', 1, 1);
INSERT INTO `realms_status` VALUES (750, 1, 'Dark', 1, 1, '2021-07-13 07:49:13', 1, 1);
INSERT INTO `realms_status` VALUES (751, 1, 'Dark', 1, 1, '2021-07-13 07:49:28', 1, 1);
INSERT INTO `realms_status` VALUES (752, 1, 'Dark', 1, 1, '2021-07-13 07:49:43', 1, 1);
INSERT INTO `realms_status` VALUES (753, 1, 'Dark', 1, 1, '2021-07-13 07:49:58', 1, 1);
INSERT INTO `realms_status` VALUES (754, 1, 'Dark', 1, 1, '2021-07-13 07:50:14', 1, 1);
INSERT INTO `realms_status` VALUES (755, 1, 'Dark', 1, 1, '2021-07-13 07:50:29', 1, 1);
INSERT INTO `realms_status` VALUES (756, 1, 'Dark', 1, 1, '2021-07-13 07:50:43', 1, 1);
INSERT INTO `realms_status` VALUES (757, 1, 'Dark', 1, 1, '2021-07-13 07:50:58', 1, 1);
INSERT INTO `realms_status` VALUES (758, 1, 'Dark', 1, 1, '2021-07-13 07:51:13', 1, 1);
INSERT INTO `realms_status` VALUES (759, 1, 'Dark', 1, 1, '2021-07-13 07:51:28', 1, 1);
INSERT INTO `realms_status` VALUES (760, 1, 'Dark', 1, 1, '2021-07-13 07:51:43', 1, 1);
INSERT INTO `realms_status` VALUES (761, 1, 'Dark', 1, 1, '2021-07-13 07:51:59', 1, 1);
INSERT INTO `realms_status` VALUES (762, 1, 'Dark', 1, 1, '2021-07-13 07:52:14', 1, 1);
INSERT INTO `realms_status` VALUES (763, 1, 'Dark', 1, 1, '2021-07-13 07:52:28', 1, 1);
INSERT INTO `realms_status` VALUES (764, 1, 'Dark', 1, 1, '2021-07-13 07:52:43', 1, 1);
INSERT INTO `realms_status` VALUES (765, 1, 'Dark', 1, 1, '2021-07-13 07:52:58', 1, 1);
INSERT INTO `realms_status` VALUES (766, 1, 'Dark', 1, 1, '2021-07-13 07:53:13', 1, 1);
INSERT INTO `realms_status` VALUES (767, 1, 'Dark', 1, 1, '2021-07-13 07:53:28', 1, 1);
INSERT INTO `realms_status` VALUES (768, 1, 'Dark', 1, 1, '2021-07-13 07:53:43', 1, 1);
INSERT INTO `realms_status` VALUES (769, 1, 'Dark', 1, 1, '2021-07-13 07:53:59', 1, 1);
INSERT INTO `realms_status` VALUES (770, 1, 'Dark', 1, 1, '2021-07-13 07:54:13', 1, 1);
INSERT INTO `realms_status` VALUES (771, 1, 'Dark', 1, 1, '2021-07-13 07:54:28', 1, 1);
INSERT INTO `realms_status` VALUES (772, 1, 'Dark', 1, 1, '2021-07-13 07:54:43', 1, 1);
INSERT INTO `realms_status` VALUES (773, 1, 'Dark', 1, 1, '2021-07-13 07:54:58', 1, 1);
INSERT INTO `realms_status` VALUES (774, 1, 'Dark', 1, 1, '2021-07-13 07:55:13', 1, 1);
INSERT INTO `realms_status` VALUES (775, 1, 'Dark', 1, 1, '2021-07-13 07:55:28', 1, 1);
INSERT INTO `realms_status` VALUES (776, 1, 'Dark', 1, 1, '2021-07-13 07:55:44', 1, 1);
INSERT INTO `realms_status` VALUES (777, 1, 'Dark', 1, 1, '2021-07-13 07:55:58', 1, 1);
INSERT INTO `realms_status` VALUES (778, 1, 'Dark', 1, 1, '2021-07-13 07:56:13', 1, 1);
INSERT INTO `realms_status` VALUES (779, 1, 'Dark', 1, 1, '2021-07-13 07:56:28', 1, 1);
INSERT INTO `realms_status` VALUES (780, 1, 'Dark', 1, 1, '2021-07-13 07:56:43', 1, 1);
INSERT INTO `realms_status` VALUES (781, 1, 'Dark', 1, 1, '2021-07-13 07:56:58', 1, 1);
INSERT INTO `realms_status` VALUES (782, 1, 'Dark', 1, 1, '2021-07-13 07:57:13', 1, 1);
INSERT INTO `realms_status` VALUES (783, 1, 'Dark', 1, 1, '2021-07-13 07:57:29', 1, 1);
INSERT INTO `realms_status` VALUES (784, 1, 'Dark', 1, 1, '2021-07-13 07:57:44', 1, 1);
INSERT INTO `realms_status` VALUES (785, 1, 'Dark', 1, 1, '2021-07-13 07:57:58', 1, 1);
INSERT INTO `realms_status` VALUES (786, 1, 'Dark', 1, 1, '2021-07-13 07:58:13', 1, 1);
INSERT INTO `realms_status` VALUES (787, 1, 'Dark', 1, 1, '2021-07-13 07:58:28', 1, 1);
INSERT INTO `realms_status` VALUES (788, 1, 'Dark', 1, 1, '2021-07-13 07:58:43', 1, 1);
INSERT INTO `realms_status` VALUES (789, 1, 'Dark', 1, 1, '2021-07-13 07:58:58', 1, 1);
INSERT INTO `realms_status` VALUES (790, 1, 'Dark', 1, 1, '2021-07-13 07:59:14', 1, 1);
INSERT INTO `realms_status` VALUES (791, 1, 'Dark', 1, 1, '2021-07-13 07:59:29', 1, 1);
INSERT INTO `realms_status` VALUES (792, 1, 'Dark', 1, 1, '2021-07-13 07:59:43', 1, 1);
INSERT INTO `realms_status` VALUES (793, 1, 'Dark', 1, 1, '2021-07-13 07:59:58', 1, 1);
INSERT INTO `realms_status` VALUES (794, 1, 'Dark', 1, 1, '2021-07-13 08:00:13', 1, 1);
INSERT INTO `realms_status` VALUES (795, 1, 'Dark', 1, 1, '2021-07-13 08:00:28', 1, 1);
INSERT INTO `realms_status` VALUES (796, 1, 'Dark', 1, 1, '2021-07-13 08:00:43', 1, 1);
INSERT INTO `realms_status` VALUES (797, 1, 'Dark', 1, 1, '2021-07-13 08:00:58', 1, 1);
INSERT INTO `realms_status` VALUES (798, 1, 'Dark', 1, 1, '2021-07-13 08:01:14', 1, 1);
INSERT INTO `realms_status` VALUES (799, 1, 'Dark', 1, 1, '2021-07-13 08:01:29', 1, 1);
INSERT INTO `realms_status` VALUES (800, 1, 'Dark', 1, 1, '2021-07-13 08:01:43', 1, 1);
INSERT INTO `realms_status` VALUES (801, 1, 'Dark', 1, 1, '2021-07-13 08:01:58', 1, 1);
INSERT INTO `realms_status` VALUES (802, 1, 'Dark', 1, 1, '2021-07-13 08:02:13', 1, 1);
INSERT INTO `realms_status` VALUES (803, 1, 'Dark', 1, 1, '2021-07-13 08:02:28', 1, 1);
INSERT INTO `realms_status` VALUES (804, 1, 'Dark', 1, 1, '2021-07-13 08:02:43', 1, 1);
INSERT INTO `realms_status` VALUES (805, 1, 'Dark', 1, 1, '2021-07-13 08:02:59', 1, 1);
INSERT INTO `realms_status` VALUES (806, 1, 'Dark', 1, 1, '2021-07-13 08:03:14', 1, 1);
INSERT INTO `realms_status` VALUES (807, 1, 'Dark', 1, 1, '2021-07-13 08:03:28', 1, 1);
INSERT INTO `realms_status` VALUES (808, 1, 'Dark', 1, 1, '2021-07-13 08:03:43', 1, 1);
INSERT INTO `realms_status` VALUES (809, 1, 'Dark', 1, 1, '2021-07-13 08:03:58', 1, 1);
INSERT INTO `realms_status` VALUES (810, 1, 'Dark', 1, 1, '2021-07-13 08:04:13', 1, 1);
INSERT INTO `realms_status` VALUES (811, 1, 'Dark', 1, 1, '2021-07-13 08:04:28', 1, 1);
INSERT INTO `realms_status` VALUES (812, 1, 'Dark', 1, 1, '2021-07-13 08:04:44', 1, 1);
INSERT INTO `realms_status` VALUES (813, 1, 'Dark', 1, 1, '2021-07-13 08:04:59', 1, 1);
INSERT INTO `realms_status` VALUES (814, 1, 'Dark', 1, 1, '2021-07-13 08:05:14', 1, 1);
INSERT INTO `realms_status` VALUES (815, 1, 'Dark', 1, 1, '2021-07-13 08:05:29', 1, 1);
INSERT INTO `realms_status` VALUES (816, 1, 'Dark', 1, 1, '2021-07-13 08:05:44', 1, 1);
INSERT INTO `realms_status` VALUES (817, 1, 'Dark', 1, 1, '2021-07-13 08:05:59', 1, 1);
INSERT INTO `realms_status` VALUES (818, 1, 'Dark', 1, 1, '2021-07-13 08:06:14', 1, 1);
INSERT INTO `realms_status` VALUES (819, 1, 'Dark', 1, 1, '2021-07-13 08:06:29', 1, 1);
INSERT INTO `realms_status` VALUES (820, 1, 'Dark', 1, 1, '2021-07-13 08:06:45', 1, 1);
INSERT INTO `realms_status` VALUES (821, 1, 'Dark', 1, 1, '2021-07-13 08:07:00', 1, 1);
INSERT INTO `realms_status` VALUES (822, 1, 'Dark', 1, 1, '2021-07-13 08:07:14', 1, 1);
INSERT INTO `realms_status` VALUES (823, 1, 'Dark', 1, 1, '2021-07-13 08:07:29', 1, 1);
INSERT INTO `realms_status` VALUES (824, 1, 'Dark', 1, 1, '2021-07-13 08:07:44', 1, 1);
INSERT INTO `realms_status` VALUES (825, 1, 'Dark', 1, 1, '2021-07-13 08:07:59', 1, 1);
INSERT INTO `realms_status` VALUES (826, 1, 'Dark', 1, 1, '2021-07-13 08:08:14', 1, 1);
INSERT INTO `realms_status` VALUES (827, 1, 'Dark', 1, 1, '2021-07-13 08:08:30', 1, 1);
INSERT INTO `realms_status` VALUES (828, 1, 'Dark', 1, 1, '2021-07-13 08:08:45', 1, 1);
INSERT INTO `realms_status` VALUES (829, 1, 'Dark', 1, 1, '2021-07-13 08:08:59', 1, 1);
INSERT INTO `realms_status` VALUES (830, 1, 'Dark', 1, 1, '2021-07-13 08:09:14', 1, 1);
INSERT INTO `realms_status` VALUES (831, 1, 'Dark', 1, 1, '2021-07-13 08:09:29', 1, 1);
INSERT INTO `realms_status` VALUES (832, 1, 'Dark', 1, 1, '2021-07-13 08:09:44', 1, 1);
INSERT INTO `realms_status` VALUES (833, 1, 'Dark', 1, 1, '2021-07-13 08:09:59', 1, 1);
INSERT INTO `realms_status` VALUES (834, 1, 'Dark', 1, 1, '2021-07-13 08:10:15', 1, 1);
INSERT INTO `realms_status` VALUES (835, 1, 'Dark', 1, 1, '2021-07-13 08:10:30', 1, 1);
INSERT INTO `realms_status` VALUES (836, 1, 'Dark', 1, 1, '2021-07-13 08:10:44', 1, 1);
INSERT INTO `realms_status` VALUES (837, 1, 'Dark', 1, 1, '2021-07-13 08:10:59', 1, 1);
INSERT INTO `realms_status` VALUES (838, 1, 'Dark', 1, 1, '2021-07-13 08:11:14', 1, 1);
INSERT INTO `realms_status` VALUES (839, 1, 'Dark', 1, 1, '2021-07-13 08:11:29', 1, 1);
INSERT INTO `realms_status` VALUES (840, 1, 'Dark', 1, 1, '2021-07-13 08:11:44', 1, 1);
INSERT INTO `realms_status` VALUES (841, 1, 'Dark', 1, 1, '2021-07-13 08:12:00', 1, 1);
INSERT INTO `realms_status` VALUES (842, 1, 'Dark', 1, 1, '2021-07-13 08:12:15', 1, 1);
INSERT INTO `realms_status` VALUES (843, 1, 'Dark', 1, 1, '2021-07-13 08:12:29', 1, 1);
INSERT INTO `realms_status` VALUES (844, 1, 'Dark', 1, 1, '2021-07-13 08:12:44', 1, 1);
INSERT INTO `realms_status` VALUES (845, 1, 'Dark', 1, 1, '2021-07-13 08:12:59', 1, 1);
INSERT INTO `realms_status` VALUES (846, 1, 'Dark', 1, 1, '2021-07-13 08:13:14', 1, 1);
INSERT INTO `realms_status` VALUES (847, 1, 'Dark', 1, 1, '2021-07-13 08:13:29', 1, 1);
INSERT INTO `realms_status` VALUES (848, 1, 'Dark', 1, 1, '2021-07-13 08:13:45', 1, 1);
INSERT INTO `realms_status` VALUES (849, 1, 'Dark', 1, 1, '2021-07-13 08:14:00', 1, 1);
INSERT INTO `realms_status` VALUES (850, 1, 'Dark', 1, 1, '2021-07-13 08:14:14', 1, 1);
INSERT INTO `realms_status` VALUES (851, 1, 'Dark', 1, 1, '2021-07-13 08:14:29', 1, 1);
INSERT INTO `realms_status` VALUES (852, 1, 'Dark', 1, 1, '2021-07-13 08:14:44', 1, 1);
INSERT INTO `realms_status` VALUES (853, 1, 'Dark', 1, 1, '2021-07-13 08:14:59', 1, 1);
INSERT INTO `realms_status` VALUES (854, 1, 'Dark', 1, 1, '2021-07-13 08:15:14', 1, 1);
INSERT INTO `realms_status` VALUES (855, 1, 'Dark', 1, 1, '2021-07-13 08:15:30', 1, 1);
INSERT INTO `realms_status` VALUES (856, 1, 'Dark', 1, 1, '2021-07-13 08:15:45', 1, 1);
INSERT INTO `realms_status` VALUES (857, 1, 'Dark', 1, 1, '2021-07-13 08:15:59', 1, 1);
INSERT INTO `realms_status` VALUES (858, 1, 'Dark', 1, 1, '2021-07-13 08:16:14', 1, 1);
INSERT INTO `realms_status` VALUES (859, 1, 'Dark', 1, 1, '2021-07-13 08:16:29', 1, 1);
INSERT INTO `realms_status` VALUES (860, 1, 'Dark', 1, 1, '2021-07-13 08:16:44', 1, 1);
INSERT INTO `realms_status` VALUES (861, 1, 'Dark', 1, 1, '2021-07-13 08:16:59', 1, 1);
INSERT INTO `realms_status` VALUES (862, 1, 'Dark', 1, 1, '2021-07-13 08:17:14', 1, 1);
INSERT INTO `realms_status` VALUES (863, 1, 'Dark', 1, 1, '2021-07-13 08:17:30', 1, 1);
INSERT INTO `realms_status` VALUES (864, 1, 'Dark', 1, 1, '2021-07-13 08:17:44', 1, 1);
INSERT INTO `realms_status` VALUES (865, 1, 'Dark', 1, 1, '2021-07-13 08:17:59', 1, 1);
INSERT INTO `realms_status` VALUES (866, 1, 'Dark', 1, 1, '2021-07-13 08:18:14', 1, 1);
INSERT INTO `realms_status` VALUES (867, 1, 'Dark', 1, 1, '2021-07-13 08:18:29', 1, 1);
INSERT INTO `realms_status` VALUES (868, 1, 'Dark', 1, 1, '2021-07-13 08:18:44', 1, 1);
INSERT INTO `realms_status` VALUES (869, 1, 'Dark', 1, 1, '2021-07-13 08:19:00', 1, 1);
INSERT INTO `realms_status` VALUES (870, 1, 'Dark', 1, 1, '2021-07-13 08:19:15', 1, 1);
INSERT INTO `realms_status` VALUES (871, 1, 'Dark', 1, 1, '2021-07-13 08:19:29', 1, 1);
INSERT INTO `realms_status` VALUES (872, 1, 'Dark', 1, 1, '2021-07-13 08:19:44', 1, 1);
INSERT INTO `realms_status` VALUES (873, 1, 'Dark', 1, 1, '2021-07-13 08:19:59', 1, 1);
INSERT INTO `realms_status` VALUES (874, 1, 'Dark', 1, 1, '2021-07-13 08:20:14', 1, 1);
INSERT INTO `realms_status` VALUES (875, 1, 'Dark', 1, 1, '2021-07-13 08:20:29', 1, 1);
INSERT INTO `realms_status` VALUES (876, 1, 'Dark', 1, 1, '2021-07-13 08:20:45', 1, 1);
INSERT INTO `realms_status` VALUES (877, 1, 'Dark', 1, 1, '2021-07-13 08:21:00', 1, 1);
INSERT INTO `realms_status` VALUES (878, 1, 'Dark', 1, 1, '2021-07-13 08:21:14', 1, 1);
INSERT INTO `realms_status` VALUES (879, 1, 'Dark', 1, 1, '2021-07-13 08:21:29', 1, 1);
INSERT INTO `realms_status` VALUES (880, 1, 'Dark', 1, 1, '2021-07-13 08:21:44', 1, 1);
INSERT INTO `realms_status` VALUES (881, 1, 'Dark', 1, 1, '2021-07-13 08:21:59', 1, 1);
INSERT INTO `realms_status` VALUES (882, 1, 'Dark', 1, 1, '2021-07-13 08:22:14', 1, 1);
INSERT INTO `realms_status` VALUES (883, 1, 'Dark', 1, 1, '2021-07-13 08:22:30', 1, 1);
INSERT INTO `realms_status` VALUES (884, 1, 'Dark', 1, 1, '2021-07-13 08:22:45', 1, 1);
INSERT INTO `realms_status` VALUES (885, 1, 'Dark', 1, 1, '2021-07-13 08:23:00', 1, 1);
INSERT INTO `realms_status` VALUES (886, 1, 'Dark', 1, 1, '2021-07-13 08:23:15', 1, 1);
INSERT INTO `realms_status` VALUES (887, 1, 'Dark', 1, 1, '2021-07-13 08:23:30', 1, 1);
INSERT INTO `realms_status` VALUES (888, 1, 'Dark', 1, 1, '2021-07-13 08:23:45', 1, 1);
INSERT INTO `realms_status` VALUES (889, 1, 'Dark', 1, 1, '2021-07-13 08:24:00', 1, 1);
INSERT INTO `realms_status` VALUES (890, 1, 'Dark', 1, 1, '2021-07-13 08:24:15', 1, 1);
INSERT INTO `realms_status` VALUES (891, 1, 'Dark', 1, 1, '2021-07-13 08:24:31', 1, 1);
INSERT INTO `realms_status` VALUES (892, 1, 'Dark', 1, 1, '2021-07-13 08:24:46', 1, 1);
INSERT INTO `realms_status` VALUES (893, 1, 'Dark', 1, 1, '2021-07-13 08:25:01', 1, 1);
INSERT INTO `realms_status` VALUES (894, 1, 'Dark', 1, 1, '2021-07-13 08:25:16', 1, 1);
INSERT INTO `realms_status` VALUES (895, 1, 'Dark', 1, 1, '2021-07-13 08:25:31', 1, 1);
INSERT INTO `realms_status` VALUES (896, 1, 'Dark', 1, 1, '2021-07-13 08:25:46', 1, 1);
INSERT INTO `realms_status` VALUES (897, 1, 'Dark', 1, 1, '2021-07-13 08:26:02', 1, 1);
INSERT INTO `realms_status` VALUES (898, 1, 'Dark', 1, 1, '2021-07-13 08:26:17', 1, 1);
INSERT INTO `realms_status` VALUES (899, 1, 'Dark', 1, 1, '2021-07-13 08:26:31', 1, 1);
INSERT INTO `realms_status` VALUES (900, 1, 'Dark', 1, 1, '2021-07-13 08:26:46', 1, 1);
INSERT INTO `realms_status` VALUES (901, 1, 'Dark', 1, 1, '2021-07-13 08:27:01', 1, 1);
INSERT INTO `realms_status` VALUES (902, 1, 'Dark', 1, 1, '2021-07-13 08:27:16', 1, 1);
INSERT INTO `realms_status` VALUES (903, 1, 'Dark', 1, 1, '2021-07-13 08:27:31', 1, 1);
INSERT INTO `realms_status` VALUES (904, 1, 'Dark', 1, 1, '2021-07-13 08:27:46', 1, 1);
INSERT INTO `realms_status` VALUES (905, 1, 'Dark', 1, 1, '2021-07-13 08:28:02', 1, 1);
INSERT INTO `realms_status` VALUES (906, 1, 'Dark', 1, 1, '2021-07-13 08:28:17', 1, 1);
INSERT INTO `realms_status` VALUES (907, 1, 'Dark', 1, 1, '2021-07-13 08:28:31', 1, 1);
INSERT INTO `realms_status` VALUES (908, 1, 'Dark', 1, 1, '2021-07-13 08:28:46', 1, 1);
INSERT INTO `realms_status` VALUES (909, 1, 'Dark', 1, 1, '2021-07-13 08:29:01', 1, 1);
INSERT INTO `realms_status` VALUES (910, 1, 'Dark', 1, 1, '2021-07-13 08:29:16', 1, 1);
INSERT INTO `realms_status` VALUES (911, 1, 'Dark', 1, 1, '2021-07-13 08:29:31', 1, 1);
INSERT INTO `realms_status` VALUES (912, 1, 'Dark', 1, 1, '2021-07-13 08:29:47', 1, 1);
INSERT INTO `realms_status` VALUES (913, 1, 'Dark', 1, 1, '2021-07-13 08:30:01', 1, 1);
INSERT INTO `realms_status` VALUES (914, 1, 'Dark', 1, 1, '2021-07-13 08:30:16', 1, 1);
INSERT INTO `realms_status` VALUES (915, 1, 'Dark', 1, 1, '2021-07-13 08:30:31', 1, 1);
INSERT INTO `realms_status` VALUES (916, 1, 'Dark', 1, 1, '2021-07-13 08:30:46', 1, 1);
INSERT INTO `realms_status` VALUES (917, 1, 'Dark', 1, 1, '2021-07-13 08:31:01', 1, 1);
INSERT INTO `realms_status` VALUES (918, 1, 'Dark', 1, 1, '2021-07-13 08:31:16', 1, 1);
INSERT INTO `realms_status` VALUES (919, 1, 'Dark', 1, 1, '2021-07-13 08:31:32', 1, 1);
INSERT INTO `realms_status` VALUES (920, 1, 'Dark', 1, 1, '2021-07-13 08:31:47', 1, 1);
INSERT INTO `realms_status` VALUES (921, 1, 'Dark', 1, 1, '2021-07-13 08:32:02', 1, 1);
INSERT INTO `realms_status` VALUES (922, 1, 'Dark', 1, 1, '2021-07-13 08:32:17', 1, 1);
INSERT INTO `realms_status` VALUES (923, 1, 'Dark', 1, 1, '2021-07-13 08:32:32', 1, 1);
INSERT INTO `realms_status` VALUES (924, 1, 'Dark', 1, 1, '2021-07-13 08:32:47', 1, 1);
INSERT INTO `realms_status` VALUES (925, 1, 'Dark', 1, 1, '2021-07-13 08:33:02', 1, 1);
INSERT INTO `realms_status` VALUES (926, 1, 'Dark', 1, 1, '2021-07-13 08:33:18', 1, 1);
INSERT INTO `realms_status` VALUES (927, 1, 'Dark', 1, 1, '2021-07-13 08:33:33', 1, 1);
INSERT INTO `realms_status` VALUES (928, 1, 'Dark', 1, 1, '2021-07-13 08:33:47', 1, 1);
INSERT INTO `realms_status` VALUES (929, 1, 'Dark', 1, 1, '2021-07-13 08:34:02', 1, 1);
INSERT INTO `realms_status` VALUES (930, 1, 'Dark', 1, 1, '2021-07-13 08:34:17', 1, 1);
INSERT INTO `realms_status` VALUES (931, 1, 'Dark', 1, 1, '2021-07-13 08:34:32', 1, 1);
INSERT INTO `realms_status` VALUES (932, 1, 'Dark', 1, 1, '2021-07-13 08:34:47', 1, 1);
INSERT INTO `realms_status` VALUES (933, 1, 'Dark', 1, 1, '2021-07-13 08:35:03', 1, 1);
INSERT INTO `realms_status` VALUES (934, 1, 'Dark', 1, 1, '2021-07-13 08:35:18', 1, 1);
INSERT INTO `realms_status` VALUES (935, 1, 'Dark', 1, 1, '2021-07-13 08:35:32', 1, 1);
INSERT INTO `realms_status` VALUES (936, 1, 'Dark', 1, 1, '2021-07-13 08:35:47', 1, 1);
INSERT INTO `realms_status` VALUES (937, 1, 'Dark', 1, 1, '2021-07-13 08:36:02', 1, 1);
INSERT INTO `realms_status` VALUES (938, 1, 'Dark', 1, 1, '2021-07-13 08:36:17', 1, 1);
INSERT INTO `realms_status` VALUES (939, 1, 'Dark', 1, 1, '2021-07-13 08:36:32', 1, 1);
INSERT INTO `realms_status` VALUES (940, 1, 'Dark', 1, 1, '2021-07-13 08:36:48', 1, 1);
INSERT INTO `realms_status` VALUES (941, 1, 'Dark', 1, 1, '2021-07-13 08:37:03', 1, 1);
INSERT INTO `realms_status` VALUES (942, 1, 'Dark', 1, 1, '2021-07-13 08:37:17', 1, 1);
INSERT INTO `realms_status` VALUES (943, 1, 'Dark', 1, 1, '2021-07-13 08:37:32', 1, 1);
INSERT INTO `realms_status` VALUES (944, 1, 'Dark', 1, 1, '2021-07-13 08:37:47', 1, 1);
INSERT INTO `realms_status` VALUES (945, 1, 'Dark', 1, 1, '2021-07-13 08:38:02', 1, 1);
INSERT INTO `realms_status` VALUES (946, 1, 'Dark', 1, 1, '2021-07-13 08:38:17', 1, 1);
INSERT INTO `realms_status` VALUES (947, 1, 'Dark', 1, 1, '2021-07-13 08:38:33', 1, 1);
INSERT INTO `realms_status` VALUES (948, 1, 'Dark', 1, 1, '2021-07-13 08:38:48', 1, 1);
INSERT INTO `realms_status` VALUES (949, 1, 'Dark', 1, 1, '2021-07-13 08:39:02', 1, 1);
INSERT INTO `realms_status` VALUES (950, 1, 'Dark', 1, 1, '2021-07-13 08:39:17', 1, 1);
INSERT INTO `realms_status` VALUES (951, 1, 'Dark', 1, 1, '2021-07-13 08:39:32', 1, 1);
INSERT INTO `realms_status` VALUES (952, 1, 'Dark', 1, 1, '2021-07-13 08:39:47', 1, 1);
INSERT INTO `realms_status` VALUES (953, 1, 'Dark', 1, 1, '2021-07-13 08:40:02', 1, 1);
INSERT INTO `realms_status` VALUES (954, 1, 'Dark', 1, 1, '2021-07-13 08:40:18', 1, 1);
INSERT INTO `realms_status` VALUES (955, 1, 'Dark', 1, 1, '2021-07-13 08:40:33', 1, 1);
INSERT INTO `realms_status` VALUES (956, 1, 'Dark', 1, 1, '2021-07-13 08:40:47', 1, 1);
INSERT INTO `realms_status` VALUES (957, 1, 'Dark', 1, 1, '2021-07-13 08:41:02', 1, 1);
INSERT INTO `realms_status` VALUES (958, 1, 'Dark', 1, 1, '2021-07-13 08:41:17', 1, 1);
INSERT INTO `realms_status` VALUES (959, 1, 'Dark', 1, 1, '2021-07-13 08:41:32', 1, 1);
INSERT INTO `realms_status` VALUES (960, 1, 'Dark', 1, 1, '2021-07-13 08:41:47', 1, 1);
INSERT INTO `realms_status` VALUES (961, 1, 'Dark', 1, 1, '2021-07-13 08:42:03', 1, 1);
INSERT INTO `realms_status` VALUES (962, 1, 'Dark', 1, 1, '2021-07-13 08:42:18', 1, 1);
INSERT INTO `realms_status` VALUES (963, 1, 'Dark', 1, 1, '2021-07-13 08:42:32', 1, 1);
INSERT INTO `realms_status` VALUES (964, 1, 'Dark', 1, 1, '2021-07-13 08:42:47', 1, 1);
INSERT INTO `realms_status` VALUES (965, 1, 'Dark', 1, 1, '2021-07-13 08:43:02', 1, 1);
INSERT INTO `realms_status` VALUES (966, 1, 'Dark', 1, 1, '2021-07-13 08:43:17', 1, 1);
INSERT INTO `realms_status` VALUES (967, 1, 'Dark', 1, 1, '2021-07-13 08:43:32', 1, 1);
INSERT INTO `realms_status` VALUES (968, 1, 'Dark', 1, 1, '2021-07-13 08:43:48', 1, 1);
INSERT INTO `realms_status` VALUES (969, 1, 'Dark', 1, 1, '2021-07-13 08:44:03', 1, 1);
INSERT INTO `realms_status` VALUES (970, 1, 'Dark', 1, 1, '2021-07-13 08:44:17', 1, 1);
INSERT INTO `realms_status` VALUES (971, 1, 'Dark', 1, 1, '2021-07-13 08:44:32', 1, 1);
INSERT INTO `realms_status` VALUES (972, 1, 'Dark', 1, 1, '2021-07-13 08:44:47', 1, 1);
INSERT INTO `realms_status` VALUES (973, 1, 'Dark', 1, 1, '2021-07-13 08:45:02', 1, 1);
INSERT INTO `realms_status` VALUES (974, 1, 'Dark', 1, 1, '2021-07-13 08:45:17', 1, 1);
INSERT INTO `realms_status` VALUES (975, 1, 'Dark', 1, 1, '2021-07-13 08:45:33', 1, 1);
INSERT INTO `realms_status` VALUES (976, 1, 'Dark', 1, 1, '2021-07-13 08:45:48', 1, 1);
INSERT INTO `realms_status` VALUES (977, 1, 'Dark', 1, 1, '2021-07-13 08:46:02', 1, 1);
INSERT INTO `realms_status` VALUES (978, 1, 'Dark', 1, 1, '2021-07-13 08:46:17', 1, 1);
INSERT INTO `realms_status` VALUES (979, 1, 'Dark', 1, 1, '2021-07-13 08:46:32', 1, 1);
INSERT INTO `realms_status` VALUES (980, 1, 'Dark', 1, 1, '2021-07-13 08:46:47', 1, 1);
INSERT INTO `realms_status` VALUES (981, 1, 'Dark', 1, 1, '2021-07-13 08:47:02', 1, 1);
INSERT INTO `realms_status` VALUES (982, 1, 'Dark', 1, 1, '2021-07-13 08:47:18', 1, 1);
INSERT INTO `realms_status` VALUES (983, 1, 'Dark', 1, 1, '2021-07-13 08:47:33', 1, 1);
INSERT INTO `realms_status` VALUES (984, 1, 'Dark', 1, 1, '2021-07-13 08:47:47', 1, 1);
INSERT INTO `realms_status` VALUES (985, 1, 'Dark', 1, 1, '2021-07-13 08:48:02', 1, 1);
INSERT INTO `realms_status` VALUES (986, 1, 'Dark', 1, 1, '2021-07-13 08:48:17', 1, 1);
INSERT INTO `realms_status` VALUES (987, 1, 'Dark', 1, 1, '2021-07-13 08:48:32', 1, 1);
INSERT INTO `realms_status` VALUES (988, 1, 'Dark', 1, 1, '2021-07-13 08:48:47', 1, 1);
INSERT INTO `realms_status` VALUES (989, 1, 'Dark', 1, 1, '2021-07-13 08:49:03', 1, 1);
INSERT INTO `realms_status` VALUES (990, 1, 'Dark', 1, 1, '2021-07-13 08:49:18', 1, 1);
INSERT INTO `realms_status` VALUES (991, 1, 'Dark', 1, 1, '2021-07-13 08:49:32', 1, 1);
INSERT INTO `realms_status` VALUES (992, 1, 'Dark', 1, 1, '2021-07-13 08:49:47', 1, 1);
INSERT INTO `realms_status` VALUES (993, 1, 'Dark', 1, 1, '2021-07-13 08:50:02', 1, 1);
INSERT INTO `realms_status` VALUES (994, 1, 'Dark', 1, 1, '2021-07-13 08:50:17', 1, 1);
INSERT INTO `realms_status` VALUES (995, 1, 'Dark', 1, 1, '2021-07-13 08:50:32', 1, 1);
INSERT INTO `realms_status` VALUES (996, 1, 'Dark', 1, 1, '2021-07-13 08:50:48', 1, 1);
INSERT INTO `realms_status` VALUES (997, 1, 'Dark', 1, 1, '2021-07-13 08:51:02', 1, 1);
INSERT INTO `realms_status` VALUES (998, 1, 'Dark', 1, 1, '2021-07-13 08:51:17', 1, 1);
INSERT INTO `realms_status` VALUES (999, 1, 'Dark', 1, 1, '2021-07-13 08:51:32', 1, 1);
INSERT INTO `realms_status` VALUES (1000, 1, 'Dark', 1, 1, '2021-07-13 08:51:47', 1, 1);
INSERT INTO `realms_status` VALUES (1001, 1, 'Dark', 1, 1, '2021-07-13 08:52:02', 1, 1);
INSERT INTO `realms_status` VALUES (1002, 1, 'Dark', 1, 1, '2021-07-13 08:52:18', 1, 1);
INSERT INTO `realms_status` VALUES (1003, 1, 'Dark', 1, 1, '2021-07-13 08:52:33', 1, 1);
INSERT INTO `realms_status` VALUES (1004, 1, 'Dark', 1, 1, '2021-07-13 08:52:47', 1, 1);
INSERT INTO `realms_status` VALUES (1005, 1, 'Dark', 1, 1, '2021-07-13 08:53:02', 1, 1);
INSERT INTO `realms_status` VALUES (1006, 1, 'Dark', 1, 1, '2021-07-13 08:53:17', 1, 1);
INSERT INTO `realms_status` VALUES (1007, 1, 'Dark', 1, 1, '2021-07-13 08:53:32', 1, 1);
INSERT INTO `realms_status` VALUES (1008, 1, 'Dark', 1, 1, '2021-07-13 08:53:47', 1, 1);
INSERT INTO `realms_status` VALUES (1009, 1, 'Dark', 1, 1, '2021-07-13 08:54:02', 1, 1);
INSERT INTO `realms_status` VALUES (1010, 1, 'Dark', 1, 1, '2021-07-13 08:54:18', 1, 1);
INSERT INTO `realms_status` VALUES (1011, 1, 'Dark', 1, 1, '2021-07-13 08:54:32', 1, 1);
INSERT INTO `realms_status` VALUES (1012, 1, 'Dark', 1, 1, '2021-07-13 08:54:47', 1, 1);
INSERT INTO `realms_status` VALUES (1013, 1, 'Dark', 1, 1, '2021-07-13 08:55:02', 1, 1);
INSERT INTO `realms_status` VALUES (1014, 1, 'Dark', 1, 1, '2021-07-13 08:55:17', 1, 1);
INSERT INTO `realms_status` VALUES (1015, 1, 'Dark', 1, 1, '2021-07-13 08:55:32', 1, 1);
INSERT INTO `realms_status` VALUES (1016, 1, 'Dark', 1, 1, '2021-07-13 08:55:48', 1, 1);
INSERT INTO `realms_status` VALUES (1017, 1, 'Dark', 1, 1, '2021-07-13 08:56:03', 1, 1);
INSERT INTO `realms_status` VALUES (1018, 1, 'Dark', 1, 1, '2021-07-13 08:56:17', 1, 1);
INSERT INTO `realms_status` VALUES (1019, 1, 'Dark', 1, 1, '2021-07-13 08:56:32', 1, 1);
INSERT INTO `realms_status` VALUES (1020, 1, 'Dark', 1, 1, '2021-07-13 08:56:47', 1, 1);
INSERT INTO `realms_status` VALUES (1021, 1, 'Dark', 1, 1, '2021-07-13 08:57:02', 1, 1);
INSERT INTO `realms_status` VALUES (1022, 1, 'Dark', 1, 1, '2021-07-13 08:57:17', 1, 1);
INSERT INTO `realms_status` VALUES (1023, 1, 'Dark', 1, 1, '2021-07-13 08:57:33', 1, 1);
INSERT INTO `realms_status` VALUES (1024, 1, 'Dark', 1, 1, '2021-07-13 08:57:48', 1, 1);
INSERT INTO `realms_status` VALUES (1025, 1, 'Dark', 1, 1, '2021-07-13 08:58:02', 1, 1);
INSERT INTO `realms_status` VALUES (1026, 1, 'Dark', 1, 1, '2021-07-13 08:58:17', 1, 1);
INSERT INTO `realms_status` VALUES (1027, 1, 'Dark', 1, 1, '2021-07-13 08:58:32', 1, 1);
INSERT INTO `realms_status` VALUES (1028, 1, 'Dark', 1, 1, '2021-07-13 08:58:47', 1, 1);
INSERT INTO `realms_status` VALUES (1029, 1, 'Dark', 1, 1, '2021-07-13 08:59:02', 1, 1);
INSERT INTO `realms_status` VALUES (1030, 1, 'Dark', 1, 1, '2021-07-13 08:59:17', 1, 1);
INSERT INTO `realms_status` VALUES (1031, 1, 'Dark', 1, 1, '2021-07-13 08:59:33', 1, 1);
INSERT INTO `realms_status` VALUES (1032, 1, 'Dark', 1, 1, '2021-07-13 08:59:48', 1, 1);
INSERT INTO `realms_status` VALUES (1033, 1, 'Dark', 1, 1, '2021-07-13 09:00:02', 1, 1);
INSERT INTO `realms_status` VALUES (1034, 1, 'Dark', 1, 1, '2021-07-13 09:00:17', 1, 1);
INSERT INTO `realms_status` VALUES (1035, 1, 'Dark', 1, 1, '2021-07-13 09:00:32', 1, 1);
INSERT INTO `realms_status` VALUES (1036, 1, 'Dark', 1, 1, '2021-07-13 09:00:47', 1, 1);
INSERT INTO `realms_status` VALUES (1037, 1, 'Dark', 1, 1, '2021-07-13 09:01:02', 1, 1);
INSERT INTO `realms_status` VALUES (1038, 1, 'Dark', 1, 1, '2021-07-13 09:01:18', 1, 1);
INSERT INTO `realms_status` VALUES (1039, 1, 'Dark', 1, 1, '2021-07-13 09:01:33', 1, 1);
INSERT INTO `realms_status` VALUES (1040, 1, 'Dark', 1, 1, '2021-07-13 09:01:47', 1, 1);
INSERT INTO `realms_status` VALUES (1041, 1, 'Dark', 1, 1, '2021-07-13 09:02:02', 1, 1);
INSERT INTO `realms_status` VALUES (1042, 1, 'Dark', 1, 1, '2021-07-13 09:02:17', 1, 1);
INSERT INTO `realms_status` VALUES (1043, 1, 'Dark', 1, 1, '2021-07-13 09:02:32', 1, 1);
INSERT INTO `realms_status` VALUES (1044, 1, 'Dark', 1, 1, '2021-07-13 09:02:47', 1, 1);
INSERT INTO `realms_status` VALUES (1045, 1, 'Dark', 1, 1, '2021-07-13 09:03:03', 1, 1);
INSERT INTO `realms_status` VALUES (1046, 1, 'Dark', 1, 1, '2021-07-13 09:03:18', 1, 1);
INSERT INTO `realms_status` VALUES (1047, 1, 'Dark', 1, 1, '2021-07-13 09:03:32', 1, 1);
INSERT INTO `realms_status` VALUES (1048, 1, 'Dark', 1, 1, '2021-07-13 09:03:47', 1, 1);
INSERT INTO `realms_status` VALUES (1049, 1, 'Dark', 1, 1, '2021-07-13 09:04:02', 1, 1);
INSERT INTO `realms_status` VALUES (1050, 1, 'Dark', 1, 1, '2021-07-13 09:04:17', 1, 1);
INSERT INTO `realms_status` VALUES (1051, 1, 'Dark', 1, 1, '2021-07-13 09:04:32', 1, 1);
INSERT INTO `realms_status` VALUES (1052, 1, 'Dark', 1, 1, '2021-07-13 09:04:48', 1, 1);
INSERT INTO `realms_status` VALUES (1053, 1, 'Dark', 1, 1, '2021-07-13 09:05:03', 1, 1);
INSERT INTO `realms_status` VALUES (1054, 1, 'Dark', 1, 1, '2021-07-13 09:05:17', 1, 1);
INSERT INTO `realms_status` VALUES (1055, 1, 'Dark', 1, 1, '2021-07-13 09:05:32', 1, 1);
INSERT INTO `realms_status` VALUES (1056, 1, 'Dark', 1, 1, '2021-07-13 09:05:47', 1, 1);
INSERT INTO `realms_status` VALUES (1057, 1, 'Dark', 1, 1, '2021-07-13 09:06:02', 1, 1);
INSERT INTO `realms_status` VALUES (1058, 1, 'Dark', 1, 1, '2021-07-13 09:06:17', 1, 1);
INSERT INTO `realms_status` VALUES (1059, 1, 'Dark', 1, 1, '2021-07-13 09:06:32', 1, 1);
INSERT INTO `realms_status` VALUES (1060, 1, 'Dark', 1, 1, '2021-07-13 09:06:48', 1, 1);
INSERT INTO `realms_status` VALUES (1061, 1, 'Dark', 1, 1, '2021-07-13 09:07:02', 1, 1);
INSERT INTO `realms_status` VALUES (1062, 1, 'Dark', 1, 1, '2021-07-13 09:07:17', 1, 1);
INSERT INTO `realms_status` VALUES (1063, 1, 'Dark', 1, 1, '2021-07-13 09:07:32', 1, 1);
INSERT INTO `realms_status` VALUES (1064, 1, 'Dark', 1, 1, '2021-07-13 09:07:47', 1, 1);
INSERT INTO `realms_status` VALUES (1065, 1, 'Dark', 1, 1, '2021-07-13 09:08:02', 1, 1);
INSERT INTO `realms_status` VALUES (1066, 1, 'Dark', 1, 1, '2021-07-13 09:08:18', 1, 1);
INSERT INTO `realms_status` VALUES (1067, 1, 'Dark', 1, 1, '2021-07-13 09:08:33', 1, 1);
INSERT INTO `realms_status` VALUES (1068, 1, 'Dark', 1, 1, '2021-07-13 09:08:47', 1, 1);
INSERT INTO `realms_status` VALUES (1069, 1, 'Dark', 1, 1, '2021-07-13 09:09:02', 1, 1);
INSERT INTO `realms_status` VALUES (1070, 1, 'Dark', 1, 1, '2021-07-13 09:09:17', 1, 1);
INSERT INTO `realms_status` VALUES (1071, 1, 'Dark', 1, 1, '2021-07-13 09:09:32', 1, 1);
INSERT INTO `realms_status` VALUES (1072, 1, 'Dark', 1, 1, '2021-07-13 09:09:47', 1, 1);
INSERT INTO `realms_status` VALUES (1073, 1, 'Dark', 1, 1, '2021-07-13 09:10:02', 1, 1);
INSERT INTO `realms_status` VALUES (1074, 1, 'Dark', 1, 1, '2021-07-13 09:10:18', 1, 1);
INSERT INTO `realms_status` VALUES (1075, 1, 'Dark', 1, 1, '2021-07-13 09:10:33', 1, 1);
INSERT INTO `realms_status` VALUES (1076, 1, 'Dark', 1, 1, '2021-07-13 09:10:47', 1, 1);
INSERT INTO `realms_status` VALUES (1077, 1, 'Dark', 1, 1, '2021-07-13 09:11:02', 1, 1);
INSERT INTO `realms_status` VALUES (1078, 1, 'Dark', 1, 1, '2021-07-13 09:11:17', 1, 1);
INSERT INTO `realms_status` VALUES (1079, 1, 'Dark', 1, 1, '2021-07-13 09:11:32', 1, 1);
INSERT INTO `realms_status` VALUES (1080, 1, 'Dark', 1, 1, '2021-07-13 09:11:48', 1, 1);
INSERT INTO `realms_status` VALUES (1081, 1, 'Dark', 1, 1, '2021-07-13 09:12:03', 1, 1);
INSERT INTO `realms_status` VALUES (1082, 1, 'Dark', 1, 1, '2021-07-13 09:12:17', 1, 1);
INSERT INTO `realms_status` VALUES (1083, 1, 'Dark', 1, 1, '2021-07-13 09:12:32', 1, 1);
INSERT INTO `realms_status` VALUES (1084, 1, 'Dark', 1, 1, '2021-07-13 09:12:47', 1, 1);
INSERT INTO `realms_status` VALUES (1085, 1, 'Dark', 1, 1, '2021-07-13 09:13:02', 1, 1);
INSERT INTO `realms_status` VALUES (1086, 1, 'Dark', 1, 1, '2021-07-13 09:13:17', 1, 1);
INSERT INTO `realms_status` VALUES (1087, 1, 'Dark', 1, 1, '2021-07-13 09:13:33', 1, 1);
INSERT INTO `realms_status` VALUES (1088, 1, 'Dark', 1, 1, '2021-07-13 09:13:48', 1, 1);
INSERT INTO `realms_status` VALUES (1089, 1, 'Dark', 1, 1, '2021-07-13 09:14:02', 1, 1);
INSERT INTO `realms_status` VALUES (1090, 1, 'Dark', 1, 1, '2021-07-13 09:14:17', 1, 1);
INSERT INTO `realms_status` VALUES (1091, 1, 'Dark', 1, 1, '2021-07-13 09:14:32', 1, 1);
INSERT INTO `realms_status` VALUES (1092, 1, 'Dark', 1, 1, '2021-07-13 09:14:47', 1, 1);
INSERT INTO `realms_status` VALUES (1093, 1, 'Dark', 1, 1, '2021-07-13 09:15:02', 1, 1);
INSERT INTO `realms_status` VALUES (1094, 1, 'Dark', 1, 1, '2021-07-13 09:15:17', 1, 1);
INSERT INTO `realms_status` VALUES (1095, 1, 'Dark', 1, 1, '2021-07-13 09:15:33', 1, 1);
INSERT INTO `realms_status` VALUES (1096, 1, 'Dark', 1, 1, '2021-07-13 09:15:48', 1, 1);
INSERT INTO `realms_status` VALUES (1097, 1, 'Dark', 1, 1, '2021-07-13 09:16:02', 1, 1);
INSERT INTO `realms_status` VALUES (1098, 1, 'Dark', 1, 1, '2021-07-13 09:16:17', 1, 1);
INSERT INTO `realms_status` VALUES (1099, 1, 'Dark', 1, 1, '2021-07-13 09:16:32', 1, 1);
INSERT INTO `realms_status` VALUES (1100, 1, 'Dark', 1, 1, '2021-07-13 09:16:47', 1, 1);
INSERT INTO `realms_status` VALUES (1101, 1, 'Dark', 1, 1, '2021-07-13 09:17:02', 1, 1);
INSERT INTO `realms_status` VALUES (1102, 1, 'Dark', 1, 1, '2021-07-13 09:17:17', 1, 1);
INSERT INTO `realms_status` VALUES (1103, 1, 'Dark', 1, 1, '2021-07-13 09:17:33', 1, 1);
INSERT INTO `realms_status` VALUES (1104, 1, 'Dark', 1, 1, '2021-07-13 09:17:47', 1, 1);
INSERT INTO `realms_status` VALUES (1105, 1, 'Dark', 1, 1, '2021-07-13 09:18:02', 1, 1);
INSERT INTO `realms_status` VALUES (1106, 1, 'Dark', 1, 1, '2021-07-13 09:18:17', 1, 1);
INSERT INTO `realms_status` VALUES (1107, 1, 'Dark', 1, 1, '2021-07-13 09:18:32', 1, 1);
INSERT INTO `realms_status` VALUES (1108, 1, 'Dark', 1, 1, '2021-07-13 09:18:47', 1, 1);
INSERT INTO `realms_status` VALUES (1109, 1, 'Dark', 1, 1, '2021-07-13 09:19:02', 1, 1);
INSERT INTO `realms_status` VALUES (1110, 1, 'Dark', 1, 1, '2021-07-13 09:19:18', 1, 1);
INSERT INTO `realms_status` VALUES (1111, 1, 'Dark', 1, 1, '2021-07-13 09:19:33', 1, 1);
INSERT INTO `realms_status` VALUES (1112, 1, 'Dark', 1, 1, '2021-07-13 09:19:48', 1, 1);
INSERT INTO `realms_status` VALUES (1113, 1, 'Dark', 1, 1, '2021-07-13 09:20:03', 1, 1);
INSERT INTO `realms_status` VALUES (1114, 1, 'Dark', 1, 1, '2021-07-13 09:20:18', 1, 1);
INSERT INTO `realms_status` VALUES (1115, 1, 'Dark', 1, 1, '2021-07-13 09:20:33', 1, 1);
INSERT INTO `realms_status` VALUES (1116, 1, 'Dark', 1, 1, '2021-07-13 09:20:48', 1, 1);
INSERT INTO `realms_status` VALUES (1117, 1, 'Dark', 1, 1, '2021-07-13 09:21:04', 1, 1);
INSERT INTO `realms_status` VALUES (1118, 1, 'Dark', 1, 1, '2021-07-13 09:21:19', 1, 1);
INSERT INTO `realms_status` VALUES (1119, 1, 'Dark', 1, 1, '2021-07-13 09:21:33', 1, 1);
INSERT INTO `realms_status` VALUES (1120, 1, 'Dark', 1, 1, '2021-07-13 09:21:48', 1, 1);
INSERT INTO `realms_status` VALUES (1121, 1, 'Dark', 1, 1, '2021-07-13 09:22:03', 1, 1);
INSERT INTO `realms_status` VALUES (1122, 1, 'Dark', 1, 1, '2021-07-13 09:22:18', 1, 1);
INSERT INTO `realms_status` VALUES (1123, 1, 'Dark', 1, 1, '2021-07-13 09:22:33', 1, 1);
INSERT INTO `realms_status` VALUES (1124, 1, 'Dark', 1, 1, '2021-07-13 09:22:49', 1, 1);
INSERT INTO `realms_status` VALUES (1125, 1, 'Dark', 1, 1, '2021-07-13 09:23:04', 1, 1);
INSERT INTO `realms_status` VALUES (1126, 1, 'Dark', 1, 1, '2021-07-13 09:23:18', 1, 1);
INSERT INTO `realms_status` VALUES (1127, 1, 'Dark', 1, 1, '2021-07-13 09:23:33', 1, 1);
INSERT INTO `realms_status` VALUES (1128, 1, 'Dark', 1, 1, '2021-07-13 09:23:48', 1, 1);
INSERT INTO `realms_status` VALUES (1129, 1, 'Dark', 1, 1, '2021-07-13 09:24:03', 1, 1);
INSERT INTO `realms_status` VALUES (1130, 1, 'Dark', 1, 1, '2021-07-13 09:24:18', 1, 1);
INSERT INTO `realms_status` VALUES (1131, 1, 'Dark', 1, 1, '2021-07-13 09:24:34', 1, 1);
INSERT INTO `realms_status` VALUES (1132, 1, 'Dark', 1, 1, '2021-07-13 09:24:49', 1, 1);
INSERT INTO `realms_status` VALUES (1133, 1, 'Dark', 1, 1, '2021-07-13 09:25:03', 1, 1);
INSERT INTO `realms_status` VALUES (1134, 1, 'Dark', 1, 1, '2021-07-13 09:25:18', 1, 1);
INSERT INTO `realms_status` VALUES (1135, 1, 'Dark', 1, 1, '2021-07-13 09:25:33', 1, 1);
INSERT INTO `realms_status` VALUES (1136, 1, 'Dark', 1, 1, '2021-07-13 09:25:48', 1, 1);
INSERT INTO `realms_status` VALUES (1137, 1, 'Dark', 1, 1, '2021-07-13 09:26:03', 1, 1);
INSERT INTO `realms_status` VALUES (1138, 1, 'Dark', 1, 1, '2021-07-13 09:26:19', 1, 1);
INSERT INTO `realms_status` VALUES (1139, 1, 'Dark', 1, 1, '2021-07-13 09:26:34', 1, 1);
INSERT INTO `realms_status` VALUES (1140, 1, 'Dark', 1, 1, '2021-07-13 09:26:48', 1, 1);
INSERT INTO `realms_status` VALUES (1141, 1, 'Dark', 1, 1, '2021-07-13 09:27:03', 1, 1);
INSERT INTO `realms_status` VALUES (1142, 1, 'Dark', 1, 1, '2021-07-13 09:27:18', 1, 1);
INSERT INTO `realms_status` VALUES (1143, 1, 'Dark', 1, 1, '2021-07-13 09:27:33', 1, 1);
INSERT INTO `realms_status` VALUES (1144, 1, 'Dark', 1, 1, '2021-07-13 09:27:48', 1, 1);
INSERT INTO `realms_status` VALUES (1145, 1, 'Dark', 1, 1, '2021-07-13 09:28:04', 1, 1);
INSERT INTO `realms_status` VALUES (1146, 1, 'Dark', 1, 1, '2021-07-13 09:28:19', 1, 1);
INSERT INTO `realms_status` VALUES (1147, 1, 'Dark', 1, 1, '2021-07-13 09:28:34', 1, 1);
INSERT INTO `realms_status` VALUES (1148, 1, 'Dark', 1, 1, '2021-07-13 09:28:48', 1, 1);
INSERT INTO `realms_status` VALUES (1149, 1, 'Dark', 1, 1, '2021-07-13 09:29:03', 1, 1);
INSERT INTO `realms_status` VALUES (1150, 1, 'Dark', 1, 1, '2021-07-13 09:29:18', 1, 1);
INSERT INTO `realms_status` VALUES (1151, 1, 'Dark', 1, 1, '2021-07-13 09:29:33', 1, 1);
INSERT INTO `realms_status` VALUES (1152, 1, 'Dark', 1, 1, '2021-07-13 09:29:48', 1, 1);
INSERT INTO `realms_status` VALUES (1153, 1, 'Dark', 1, 1, '2021-07-13 09:30:04', 1, 1);
INSERT INTO `realms_status` VALUES (1154, 1, 'Dark', 1, 1, '2021-07-13 09:30:19', 1, 1);
INSERT INTO `realms_status` VALUES (1155, 1, 'Dark', 1, 1, '2021-07-13 09:30:33', 1, 1);
INSERT INTO `realms_status` VALUES (1156, 1, 'Dark', 1, 1, '2021-07-13 09:30:48', 1, 1);
INSERT INTO `realms_status` VALUES (1157, 1, 'Dark', 1, 1, '2021-07-13 09:31:03', 1, 1);
INSERT INTO `realms_status` VALUES (1158, 1, 'Dark', 1, 1, '2021-07-13 09:31:18', 1, 1);
INSERT INTO `realms_status` VALUES (1159, 1, 'Dark', 1, 1, '2021-07-13 09:31:33', 1, 1);
INSERT INTO `realms_status` VALUES (1160, 1, 'Dark', 1, 1, '2021-07-13 09:31:49', 1, 1);
INSERT INTO `realms_status` VALUES (1161, 1, 'Dark', 1, 1, '2021-07-13 09:32:04', 1, 1);
INSERT INTO `realms_status` VALUES (1162, 1, 'Dark', 1, 1, '2021-07-13 09:32:18', 1, 1);
INSERT INTO `realms_status` VALUES (1163, 1, 'Dark', 1, 1, '2021-07-13 09:32:33', 1, 1);
INSERT INTO `realms_status` VALUES (1164, 1, 'Dark', 1, 1, '2021-07-13 09:32:48', 1, 1);
INSERT INTO `realms_status` VALUES (1165, 1, 'Dark', 1, 1, '2021-07-13 09:33:03', 1, 1);
INSERT INTO `realms_status` VALUES (1166, 1, 'Dark', 1, 1, '2021-07-13 09:33:18', 1, 1);
INSERT INTO `realms_status` VALUES (1167, 1, 'Dark', 1, 1, '2021-07-13 09:33:33', 1, 1);
INSERT INTO `realms_status` VALUES (1168, 1, 'Dark', 1, 1, '2021-07-13 09:33:49', 1, 1);
INSERT INTO `realms_status` VALUES (1169, 1, 'Dark', 1, 1, '2021-07-13 09:34:04', 1, 1);
INSERT INTO `realms_status` VALUES (1170, 1, 'Dark', 1, 1, '2021-07-13 09:34:18', 1, 1);
INSERT INTO `realms_status` VALUES (1171, 1, 'Dark', 1, 1, '2021-07-13 09:34:33', 1, 1);
INSERT INTO `realms_status` VALUES (1172, 1, 'Dark', 1, 1, '2021-07-13 09:34:48', 1, 1);
INSERT INTO `realms_status` VALUES (1173, 1, 'Dark', 1, 1, '2021-07-13 09:35:03', 1, 1);
INSERT INTO `realms_status` VALUES (1174, 1, 'Dark', 1, 1, '2021-07-13 09:35:18', 1, 1);
INSERT INTO `realms_status` VALUES (1175, 1, 'Dark', 1, 1, '2021-07-13 09:35:34', 1, 1);
INSERT INTO `realms_status` VALUES (1176, 1, 'Dark', 1, 1, '2021-07-13 09:35:49', 1, 1);
INSERT INTO `realms_status` VALUES (1177, 1, 'Dark', 1, 1, '2021-07-13 09:36:03', 1, 1);
INSERT INTO `realms_status` VALUES (1178, 1, 'Dark', 1, 1, '2021-07-13 09:36:18', 1, 1);
INSERT INTO `realms_status` VALUES (1179, 1, 'Dark', 1, 1, '2021-07-13 09:36:33', 1, 1);
INSERT INTO `realms_status` VALUES (1180, 1, 'Dark', 1, 1, '2021-07-13 09:36:48', 1, 1);
INSERT INTO `realms_status` VALUES (1181, 1, 'Dark', 1, 1, '2021-07-13 09:37:03', 1, 1);
INSERT INTO `realms_status` VALUES (1182, 1, 'Dark', 1, 1, '2021-07-13 09:37:19', 1, 1);
INSERT INTO `realms_status` VALUES (1183, 1, 'Dark', 1, 1, '2021-07-13 09:37:34', 1, 1);
INSERT INTO `realms_status` VALUES (1184, 1, 'Dark', 1, 1, '2021-07-13 09:37:48', 1, 1);
INSERT INTO `realms_status` VALUES (1185, 1, 'Dark', 1, 1, '2021-07-13 09:38:03', 1, 1);
INSERT INTO `realms_status` VALUES (1186, 1, 'Dark', 1, 1, '2021-07-13 09:38:18', 1, 1);
INSERT INTO `realms_status` VALUES (1187, 1, 'Dark', 1, 1, '2021-07-13 09:38:33', 1, 1);
INSERT INTO `realms_status` VALUES (1188, 1, 'Dark', 1, 1, '2021-07-13 09:38:48', 1, 1);
INSERT INTO `realms_status` VALUES (1189, 1, 'Dark', 1, 1, '2021-07-13 09:39:04', 1, 1);
INSERT INTO `realms_status` VALUES (1190, 1, 'Dark', 1, 1, '2021-07-13 09:39:19', 1, 1);
INSERT INTO `realms_status` VALUES (1191, 1, 'Dark', 1, 1, '2021-07-13 09:39:33', 1, 1);
INSERT INTO `realms_status` VALUES (1192, 1, 'Dark', 1, 1, '2021-07-13 09:39:48', 1, 1);
INSERT INTO `realms_status` VALUES (1193, 1, 'Dark', 1, 1, '2021-07-13 09:40:03', 1, 1);
INSERT INTO `realms_status` VALUES (1194, 1, 'Dark', 1, 1, '2021-07-13 09:40:18', 1, 1);
INSERT INTO `realms_status` VALUES (1195, 1, 'Dark', 1, 1, '2021-07-13 09:40:33', 1, 1);
INSERT INTO `realms_status` VALUES (1196, 1, 'Dark', 1, 1, '2021-07-13 09:40:49', 1, 1);
INSERT INTO `realms_status` VALUES (1197, 1, 'Dark', 1, 1, '2021-07-13 09:41:04', 1, 1);
INSERT INTO `realms_status` VALUES (1198, 1, 'Dark', 1, 1, '2021-07-13 09:41:18', 1, 1);
INSERT INTO `realms_status` VALUES (1199, 1, 'Dark', 1, 1, '2021-07-13 09:41:33', 1, 1);
INSERT INTO `realms_status` VALUES (1200, 1, 'Dark', 1, 1, '2021-07-13 09:41:48', 1, 1);
INSERT INTO `realms_status` VALUES (1201, 1, 'Dark', 1, 1, '2021-07-13 09:42:03', 1, 1);
INSERT INTO `realms_status` VALUES (1202, 1, 'Dark', 1, 1, '2021-07-13 09:42:18', 1, 1);
INSERT INTO `realms_status` VALUES (1203, 1, 'Dark', 1, 1, '2021-07-13 09:42:34', 1, 1);
INSERT INTO `realms_status` VALUES (1204, 1, 'Dark', 1, 1, '2021-07-13 09:42:49', 1, 1);
INSERT INTO `realms_status` VALUES (1205, 1, 'Dark', 1, 1, '2021-07-13 09:43:04', 1, 1);
INSERT INTO `realms_status` VALUES (1206, 1, 'Dark', 1, 1, '2021-07-13 09:43:19', 1, 1);
INSERT INTO `realms_status` VALUES (1207, 1, 'Dark', 1, 1, '2021-07-13 09:43:34', 1, 1);
INSERT INTO `realms_status` VALUES (1208, 1, 'Dark', 1, 1, '2021-07-13 09:43:49', 1, 1);
INSERT INTO `realms_status` VALUES (1209, 1, 'Dark', 1, 1, '2021-07-13 09:44:04', 1, 1);
INSERT INTO `realms_status` VALUES (1210, 1, 'Dark', 1, 1, '2021-07-13 09:44:20', 1, 1);
INSERT INTO `realms_status` VALUES (1211, 1, 'Dark', 1, 1, '2021-07-13 09:44:35', 1, 1);
INSERT INTO `realms_status` VALUES (1212, 1, 'Dark', 1, 1, '2021-07-13 09:44:49', 1, 1);
INSERT INTO `realms_status` VALUES (1213, 1, 'Dark', 1, 1, '2021-07-13 09:45:04', 1, 1);
INSERT INTO `realms_status` VALUES (1214, 1, 'Dark', 1, 1, '2021-07-13 09:45:19', 1, 1);
INSERT INTO `realms_status` VALUES (1215, 1, 'Dark', 1, 1, '2021-07-13 09:45:34', 1, 1);
INSERT INTO `realms_status` VALUES (1216, 1, 'Dark', 1, 1, '2021-07-13 09:45:49', 1, 1);
INSERT INTO `realms_status` VALUES (1217, 1, 'Dark', 1, 1, '2021-07-13 09:46:05', 1, 1);
INSERT INTO `realms_status` VALUES (1218, 1, 'Dark', 1, 1, '2021-07-13 09:46:20', 1, 1);
INSERT INTO `realms_status` VALUES (1219, 1, 'Dark', 1, 1, '2021-07-13 09:46:34', 1, 1);
INSERT INTO `realms_status` VALUES (1220, 1, 'Dark', 1, 1, '2021-07-13 09:46:49', 1, 1);
INSERT INTO `realms_status` VALUES (1221, 1, 'Dark', 1, 1, '2021-07-13 09:47:04', 1, 1);
INSERT INTO `realms_status` VALUES (1222, 1, 'Dark', 1, 1, '2021-07-13 09:47:19', 1, 1);
INSERT INTO `realms_status` VALUES (1223, 1, 'Dark', 1, 1, '2021-07-13 09:47:34', 1, 1);
INSERT INTO `realms_status` VALUES (1224, 1, 'Dark', 1, 1, '2021-07-13 09:47:50', 1, 1);
INSERT INTO `realms_status` VALUES (1225, 1, 'Dark', 1, 1, '2021-07-13 09:48:05', 1, 1);
INSERT INTO `realms_status` VALUES (1226, 1, 'Dark', 1, 1, '2021-07-13 09:48:19', 1, 1);
INSERT INTO `realms_status` VALUES (1227, 1, 'Dark', 1, 1, '2021-07-13 09:48:34', 1, 1);
INSERT INTO `realms_status` VALUES (1228, 1, 'Dark', 1, 1, '2021-07-13 09:48:49', 1, 1);
INSERT INTO `realms_status` VALUES (1229, 1, 'Dark', 1, 1, '2021-07-13 09:49:04', 1, 1);
INSERT INTO `realms_status` VALUES (1230, 1, 'Dark', 1, 1, '2021-07-13 09:49:19', 1, 1);
INSERT INTO `realms_status` VALUES (1231, 1, 'Dark', 1, 1, '2021-07-13 09:49:35', 1, 1);
INSERT INTO `realms_status` VALUES (1232, 1, 'Dark', 1, 1, '2021-07-13 09:49:50', 1, 1);
INSERT INTO `realms_status` VALUES (1233, 1, 'Dark', 1, 1, '2021-07-13 09:50:04', 1, 1);
INSERT INTO `realms_status` VALUES (1234, 1, 'Dark', 1, 1, '2021-07-13 09:50:19', 1, 1);
INSERT INTO `realms_status` VALUES (1235, 1, 'Dark', 1, 1, '2021-07-13 09:50:34', 1, 1);
INSERT INTO `realms_status` VALUES (1236, 1, 'Dark', 1, 1, '2021-07-13 09:50:49', 1, 1);
INSERT INTO `realms_status` VALUES (1237, 1, 'Dark', 1, 1, '2021-07-13 09:51:05', 1, 1);
INSERT INTO `realms_status` VALUES (1238, 1, 'Dark', 1, 1, '2021-07-13 09:51:20', 1, 1);
INSERT INTO `realms_status` VALUES (1239, 1, 'Dark', 1, 1, '2021-07-13 09:51:34', 1, 1);
INSERT INTO `realms_status` VALUES (1240, 1, 'Dark', 1, 1, '2021-07-13 09:51:49', 1, 1);
INSERT INTO `realms_status` VALUES (1241, 1, 'Dark', 1, 1, '2021-07-13 09:52:04', 1, 1);
INSERT INTO `realms_status` VALUES (1242, 1, 'Dark', 1, 1, '2021-07-13 09:52:19', 1, 1);
INSERT INTO `realms_status` VALUES (1243, 1, 'Dark', 1, 1, '2021-07-13 09:52:34', 1, 1);
INSERT INTO `realms_status` VALUES (1244, 1, 'Dark', 1, 1, '2021-07-13 09:52:50', 1, 1);
INSERT INTO `realms_status` VALUES (1245, 1, 'Dark', 1, 1, '2021-07-13 09:53:05', 1, 1);
INSERT INTO `realms_status` VALUES (1246, 1, 'Dark', 1, 1, '2021-07-13 09:53:20', 1, 1);
INSERT INTO `realms_status` VALUES (1247, 1, 'Dark', 1, 1, '2021-07-13 09:53:35', 1, 1);
INSERT INTO `realms_status` VALUES (1248, 1, 'Dark', 1, 1, '2021-07-13 09:53:50', 1, 1);
INSERT INTO `realms_status` VALUES (1249, 1, 'Dark', 1, 1, '2021-07-13 09:54:05', 1, 1);
INSERT INTO `realms_status` VALUES (1250, 1, 'Dark', 1, 1, '2021-07-13 09:54:21', 1, 1);
INSERT INTO `realms_status` VALUES (1251, 1, 'Dark', 1, 1, '2021-07-13 09:54:36', 1, 1);
INSERT INTO `realms_status` VALUES (1252, 1, 'Dark', 1, 1, '2021-07-13 09:54:50', 1, 1);
INSERT INTO `realms_status` VALUES (1253, 1, 'Dark', 1, 1, '2021-07-13 09:55:05', 1, 1);
INSERT INTO `realms_status` VALUES (1254, 1, 'Dark', 1, 1, '2021-07-13 09:55:20', 1, 1);
INSERT INTO `realms_status` VALUES (1255, 1, 'Dark', 1, 1, '2021-07-13 09:55:35', 1, 1);
INSERT INTO `realms_status` VALUES (1256, 1, 'Dark', 1, 1, '2021-07-13 09:55:50', 1, 1);
INSERT INTO `realms_status` VALUES (1257, 1, 'Dark', 1, 1, '2021-07-13 09:56:06', 1, 1);
INSERT INTO `realms_status` VALUES (1258, 1, 'Dark', 1, 1, '2021-07-13 09:56:21', 1, 1);
INSERT INTO `realms_status` VALUES (1259, 1, 'Dark', 1, 1, '2021-07-13 09:56:35', 1, 1);
INSERT INTO `realms_status` VALUES (1260, 1, 'Dark', 1, 1, '2021-07-13 09:56:50', 1, 1);
INSERT INTO `realms_status` VALUES (1261, 1, 'Dark', 1, 1, '2021-07-13 09:57:05', 1, 1);
INSERT INTO `realms_status` VALUES (1262, 1, 'Dark', 1, 1, '2021-07-13 09:57:20', 1, 1);
INSERT INTO `realms_status` VALUES (1263, 1, 'Dark', 1, 1, '2021-07-13 09:57:35', 1, 1);
INSERT INTO `realms_status` VALUES (1264, 1, 'Dark', 1, 1, '2021-07-13 09:57:51', 1, 1);
INSERT INTO `realms_status` VALUES (1265, 1, 'Dark', 1, 1, '2021-07-13 09:58:06', 1, 1);
INSERT INTO `realms_status` VALUES (1266, 1, 'Dark', 1, 1, '2021-07-13 09:58:20', 1, 1);
INSERT INTO `realms_status` VALUES (1267, 1, 'Dark', 1, 1, '2021-07-13 09:58:35', 1, 1);
INSERT INTO `realms_status` VALUES (1268, 1, 'Dark', 1, 1, '2021-07-13 09:58:50', 1, 1);
INSERT INTO `realms_status` VALUES (1269, 1, 'Dark', 1, 1, '2021-07-13 09:59:05', 1, 1);
INSERT INTO `realms_status` VALUES (1270, 1, 'Dark', 1, 1, '2021-07-13 09:59:20', 1, 1);
INSERT INTO `realms_status` VALUES (1271, 1, 'Dark', 1, 1, '2021-07-13 09:59:36', 1, 1);
INSERT INTO `realms_status` VALUES (1272, 1, 'Dark', 1, 1, '2021-07-13 09:59:51', 1, 1);
INSERT INTO `realms_status` VALUES (1273, 1, 'Dark', 1, 1, '2021-07-13 10:00:05', 1, 1);
INSERT INTO `realms_status` VALUES (1274, 1, 'Dark', 1, 1, '2021-07-13 10:00:20', 1, 1);
INSERT INTO `realms_status` VALUES (1275, 1, 'Dark', 1, 1, '2021-07-13 10:00:35', 1, 1);
INSERT INTO `realms_status` VALUES (1276, 1, 'Dark', 1, 1, '2021-07-13 10:00:50', 1, 1);
INSERT INTO `realms_status` VALUES (1277, 1, 'Dark', 1, 1, '2021-07-13 10:01:05', 1, 1);
INSERT INTO `realms_status` VALUES (1278, 1, 'Dark', 1, 1, '2021-07-13 10:01:20', 1, 1);
INSERT INTO `realms_status` VALUES (1279, 1, 'Dark', 1, 1, '2021-07-13 10:01:36', 1, 1);
INSERT INTO `realms_status` VALUES (1280, 1, 'Dark', 1, 1, '2021-07-13 10:01:50', 1, 1);
INSERT INTO `realms_status` VALUES (1281, 1, 'Dark', 1, 1, '2021-07-13 10:02:05', 1, 1);
INSERT INTO `realms_status` VALUES (1282, 1, 'Dark', 1, 1, '2021-07-13 10:02:20', 1, 1);
INSERT INTO `realms_status` VALUES (1283, 1, 'Dark', 1, 1, '2021-07-13 10:02:35', 1, 1);
INSERT INTO `realms_status` VALUES (1284, 1, 'Dark', 1, 1, '2021-07-13 10:02:50', 1, 1);
INSERT INTO `realms_status` VALUES (1285, 1, 'Dark', 1, 1, '2021-07-13 10:03:05', 1, 1);
INSERT INTO `realms_status` VALUES (1286, 1, 'Dark', 1, 1, '2021-07-13 10:03:21', 1, 1);
INSERT INTO `realms_status` VALUES (1287, 1, 'Dark', 1, 1, '2021-07-13 10:03:36', 1, 1);
INSERT INTO `realms_status` VALUES (1288, 1, 'Dark', 1, 1, '2021-07-13 10:03:50', 1, 1);
INSERT INTO `realms_status` VALUES (1289, 1, 'Dark', 1, 1, '2021-07-13 10:04:05', 1, 1);
INSERT INTO `realms_status` VALUES (1290, 1, 'Dark', 1, 1, '2021-07-13 10:04:20', 1, 1);
INSERT INTO `realms_status` VALUES (1291, 1, 'Dark', 1, 1, '2021-07-13 10:04:35', 1, 1);
INSERT INTO `realms_status` VALUES (1292, 1, 'Dark', 1, 1, '2021-07-13 10:04:50', 1, 1);
INSERT INTO `realms_status` VALUES (1293, 1, 'Dark', 1, 1, '2021-07-13 10:05:06', 1, 1);
INSERT INTO `realms_status` VALUES (1294, 1, 'Dark', 1, 1, '2021-07-13 10:05:21', 1, 1);
INSERT INTO `realms_status` VALUES (1295, 1, 'Dark', 1, 1, '2021-07-13 10:05:35', 1, 1);
INSERT INTO `realms_status` VALUES (1296, 1, 'Dark', 1, 1, '2021-07-13 10:05:50', 1, 1);
INSERT INTO `realms_status` VALUES (1297, 1, 'Dark', 1, 1, '2021-07-13 10:06:05', 1, 1);
INSERT INTO `realms_status` VALUES (1298, 1, 'Dark', 1, 1, '2021-07-13 10:06:20', 1, 1);
INSERT INTO `realms_status` VALUES (1299, 1, 'Dark', 1, 1, '2021-07-13 10:06:35', 1, 1);
INSERT INTO `realms_status` VALUES (1300, 1, 'Dark', 1, 1, '2021-07-13 10:06:51', 1, 1);
INSERT INTO `realms_status` VALUES (1301, 1, 'Dark', 1, 1, '2021-07-13 10:07:06', 1, 1);
INSERT INTO `realms_status` VALUES (1302, 1, 'Dark', 1, 1, '2021-07-13 10:07:20', 1, 1);
INSERT INTO `realms_status` VALUES (1303, 1, 'Dark', 1, 1, '2021-07-13 10:07:35', 1, 1);
INSERT INTO `realms_status` VALUES (1304, 1, 'Dark', 1, 1, '2021-07-13 10:07:50', 1, 1);
INSERT INTO `realms_status` VALUES (1305, 1, 'Dark', 1, 1, '2021-07-13 10:08:05', 1, 1);
INSERT INTO `realms_status` VALUES (1306, 1, 'Dark', 1, 1, '2021-07-13 10:08:20', 1, 1);
INSERT INTO `realms_status` VALUES (1307, 1, 'Dark', 1, 1, '2021-07-13 10:08:35', 1, 1);
INSERT INTO `realms_status` VALUES (1308, 1, 'Dark', 1, 1, '2021-07-13 10:08:51', 1, 1);
INSERT INTO `realms_status` VALUES (1309, 1, 'Dark', 1, 1, '2021-07-13 10:09:05', 1, 1);
INSERT INTO `realms_status` VALUES (1310, 1, 'Dark', 1, 1, '2021-07-13 10:09:20', 1, 1);
INSERT INTO `realms_status` VALUES (1311, 1, 'Dark', 1, 1, '2021-07-13 10:09:35', 1, 1);
INSERT INTO `realms_status` VALUES (1312, 1, 'Dark', 1, 1, '2021-07-13 10:09:50', 1, 1);
INSERT INTO `realms_status` VALUES (1313, 1, 'Dark', 1, 1, '2021-07-13 10:10:05', 1, 1);
INSERT INTO `realms_status` VALUES (1314, 1, 'Dark', 1, 1, '2021-07-13 10:10:21', 1, 1);
INSERT INTO `realms_status` VALUES (1315, 1, 'Dark', 1, 1, '2021-07-13 10:10:36', 1, 1);
INSERT INTO `realms_status` VALUES (1316, 1, 'Dark', 1, 1, '2021-07-13 10:10:51', 1, 1);
INSERT INTO `realms_status` VALUES (1317, 1, 'Dark', 1, 1, '2021-07-13 10:11:06', 1, 1);
INSERT INTO `realms_status` VALUES (1318, 1, 'Dark', 1, 1, '2021-07-13 10:11:21', 1, 1);
INSERT INTO `realms_status` VALUES (1319, 1, 'Dark', 1, 1, '2021-07-13 10:11:36', 1, 1);
INSERT INTO `realms_status` VALUES (1320, 1, 'Dark', 1, 1, '2021-07-13 10:11:51', 1, 1);
INSERT INTO `realms_status` VALUES (1321, 1, 'Dark', 1, 1, '2021-07-13 10:12:06', 1, 1);
INSERT INTO `realms_status` VALUES (1322, 1, 'Dark', 1, 1, '2021-07-13 10:12:22', 1, 1);
INSERT INTO `realms_status` VALUES (1323, 1, 'Dark', 1, 1, '2021-07-13 10:12:37', 1, 1);
INSERT INTO `realms_status` VALUES (1324, 1, 'Dark', 1, 1, '2021-07-13 10:12:51', 1, 1);
INSERT INTO `realms_status` VALUES (1325, 1, 'Dark', 1, 1, '2021-07-13 10:13:06', 1, 1);
INSERT INTO `realms_status` VALUES (1326, 1, 'Dark', 1, 1, '2021-07-13 10:13:21', 1, 1);
INSERT INTO `realms_status` VALUES (1327, 1, 'Dark', 1, 1, '2021-07-13 10:13:36', 1, 1);
INSERT INTO `realms_status` VALUES (1328, 1, 'Dark', 1, 1, '2021-07-13 10:13:51', 1, 1);
INSERT INTO `realms_status` VALUES (1329, 1, 'Dark', 1, 1, '2021-07-13 10:14:07', 1, 1);
INSERT INTO `realms_status` VALUES (1330, 1, 'Dark', 1, 1, '2021-07-13 10:14:22', 1, 1);
INSERT INTO `realms_status` VALUES (1331, 1, 'Dark', 1, 1, '2021-07-13 10:14:36', 1, 1);
INSERT INTO `realms_status` VALUES (1332, 1, 'Dark', 1, 1, '2021-07-13 10:14:51', 1, 1);
INSERT INTO `realms_status` VALUES (1333, 1, 'Dark', 1, 1, '2021-07-13 10:15:06', 1, 1);
INSERT INTO `realms_status` VALUES (1334, 1, 'Dark', 1, 1, '2021-07-13 10:15:21', 1, 1);
INSERT INTO `realms_status` VALUES (1335, 1, 'Dark', 1, 1, '2021-07-13 10:15:36', 1, 1);
INSERT INTO `realms_status` VALUES (1336, 1, 'Dark', 1, 1, '2021-07-13 10:15:52', 1, 1);
INSERT INTO `realms_status` VALUES (1337, 1, 'Dark', 1, 1, '2021-07-13 10:16:07', 1, 1);
INSERT INTO `realms_status` VALUES (1338, 1, 'Dark', 1, 1, '2021-07-13 10:16:21', 1, 1);
INSERT INTO `realms_status` VALUES (1339, 1, 'Dark', 1, 1, '2021-07-13 10:16:36', 1, 1);
INSERT INTO `realms_status` VALUES (1340, 1, 'Dark', 1, 1, '2021-07-13 10:16:51', 1, 1);
INSERT INTO `realms_status` VALUES (1341, 1, 'Dark', 1, 1, '2021-07-13 10:17:06', 1, 1);
INSERT INTO `realms_status` VALUES (1342, 1, 'Dark', 1, 1, '2021-07-13 10:17:21', 1, 1);
INSERT INTO `realms_status` VALUES (1343, 1, 'Dark', 1, 1, '2021-07-13 10:17:36', 1, 1);
INSERT INTO `realms_status` VALUES (1344, 1, 'Dark', 1, 1, '2021-07-13 10:17:52', 1, 1);
INSERT INTO `realms_status` VALUES (1345, 1, 'Dark', 1, 1, '2021-07-13 10:18:06', 1, 1);
INSERT INTO `realms_status` VALUES (1346, 1, 'Dark', 1, 1, '2021-07-13 10:18:21', 1, 1);
INSERT INTO `realms_status` VALUES (1347, 1, 'Dark', 1, 1, '2021-07-13 10:18:36', 1, 1);
INSERT INTO `realms_status` VALUES (1348, 1, 'Dark', 1, 1, '2021-07-13 10:18:51', 1, 1);
INSERT INTO `realms_status` VALUES (1349, 1, 'Dark', 1, 1, '2021-07-13 10:19:06', 1, 1);
INSERT INTO `realms_status` VALUES (1350, 1, 'Dark', 1, 1, '2021-07-13 10:19:22', 1, 1);
INSERT INTO `realms_status` VALUES (1351, 1, 'Dark', 1, 1, '2021-07-13 10:19:37', 1, 1);
INSERT INTO `realms_status` VALUES (1352, 1, 'Dark', 1, 1, '2021-07-13 10:19:51', 1, 1);
INSERT INTO `realms_status` VALUES (1353, 1, 'Dark', 1, 1, '2021-07-13 10:20:06', 1, 1);
INSERT INTO `realms_status` VALUES (1354, 1, 'Dark', 1, 1, '2021-07-13 10:20:21', 1, 1);
INSERT INTO `realms_status` VALUES (1355, 1, 'Dark', 1, 1, '2021-07-13 10:20:36', 1, 1);
INSERT INTO `realms_status` VALUES (1356, 1, 'Dark', 1, 1, '2021-07-13 11:10:10', 0, 0);
INSERT INTO `realms_status` VALUES (1357, 1, 'Dark', 1, 1, '2021-07-13 11:10:25', 0, 0);
INSERT INTO `realms_status` VALUES (1358, 1, 'Dark', 1, 1, '2021-07-13 11:10:41', 1, 1);
INSERT INTO `realms_status` VALUES (1359, 1, 'Dark', 1, 1, '2021-07-13 11:10:55', 1, 1);
INSERT INTO `realms_status` VALUES (1360, 1, 'Dark', 1, 1, '2021-07-13 11:11:10', 1, 1);
INSERT INTO `realms_status` VALUES (1361, 1, 'Dark', 1, 1, '2021-07-13 11:11:25', 1, 1);
INSERT INTO `realms_status` VALUES (1362, 1, 'Dark', 1, 1, '2021-07-13 11:11:40', 1, 1);
INSERT INTO `realms_status` VALUES (1363, 1, 'Dark', 1, 1, '2021-07-13 11:11:55', 1, 1);
INSERT INTO `realms_status` VALUES (1364, 1, 'Dark', 1, 1, '2021-07-13 11:12:11', 1, 1);
INSERT INTO `realms_status` VALUES (1365, 1, 'Dark', 1, 1, '2021-07-13 11:12:25', 1, 1);
INSERT INTO `realms_status` VALUES (1366, 1, 'Dark', 1, 1, '2021-07-13 11:12:40', 1, 1);
INSERT INTO `realms_status` VALUES (1367, 1, 'Dark', 1, 1, '2021-07-13 11:12:55', 1, 1);
INSERT INTO `realms_status` VALUES (1368, 1, 'Dark', 1, 1, '2021-07-13 11:13:10', 1, 1);
INSERT INTO `realms_status` VALUES (1369, 1, 'Dark', 1, 1, '2021-07-13 11:13:25', 1, 1);
INSERT INTO `realms_status` VALUES (1370, 1, 'Dark', 1, 1, '2021-07-13 11:13:41', 1, 1);
INSERT INTO `realms_status` VALUES (1371, 1, 'Dark', 1, 1, '2021-07-13 11:13:56', 1, 1);
INSERT INTO `realms_status` VALUES (1372, 1, 'Dark', 1, 1, '2021-07-13 11:14:10', 1, 1);
INSERT INTO `realms_status` VALUES (1373, 1, 'Dark', 1, 1, '2021-07-13 11:14:25', 1, 1);
INSERT INTO `realms_status` VALUES (1374, 1, 'Dark', 1, 1, '2021-07-13 11:14:40', 1, 1);
INSERT INTO `realms_status` VALUES (1375, 1, 'Dark', 1, 1, '2021-07-13 11:14:55', 1, 1);
INSERT INTO `realms_status` VALUES (1376, 1, 'Dark', 1, 1, '2021-07-13 11:15:10', 1, 1);
INSERT INTO `realms_status` VALUES (1377, 1, 'Dark', 1, 1, '2021-07-13 11:15:25', 1, 1);
INSERT INTO `realms_status` VALUES (1378, 1, 'Dark', 1, 1, '2021-07-13 11:15:41', 1, 1);
INSERT INTO `realms_status` VALUES (1379, 1, 'Dark', 1, 1, '2021-07-13 11:15:56', 1, 1);
INSERT INTO `realms_status` VALUES (1380, 1, 'Dark', 1, 1, '2021-07-13 11:16:11', 1, 1);
INSERT INTO `realms_status` VALUES (1381, 1, 'Dark', 1, 1, '2021-07-13 11:16:26', 1, 1);
INSERT INTO `realms_status` VALUES (1382, 1, 'Dark', 1, 1, '2021-07-13 11:16:41', 1, 1);
INSERT INTO `realms_status` VALUES (1383, 1, 'Dark', 1, 1, '2021-07-13 11:16:56', 1, 1);
INSERT INTO `realms_status` VALUES (1384, 1, 'Dark', 1, 1, '2021-07-13 11:17:12', 1, 1);
INSERT INTO `realms_status` VALUES (1385, 1, 'Dark', 1, 1, '2021-07-13 11:18:02', 1, 1);
INSERT INTO `realms_status` VALUES (1386, 1, 'Dark', 1, 1, '2021-07-13 11:18:17', 1, 1);
INSERT INTO `realms_status` VALUES (1387, 1, 'Dark', 1, 1, '2021-07-13 11:18:56', 0, 0);
INSERT INTO `realms_status` VALUES (1388, 1, 'Dark', 1, 1, '2021-07-13 11:19:12', 0, 0);
INSERT INTO `realms_status` VALUES (1389, 1, 'Dark', 1, 1, '2021-07-13 11:19:27', 0, 0);
INSERT INTO `realms_status` VALUES (1390, 1, 'Dark', 1, 1, '2021-07-13 11:19:41', 0, 0);
INSERT INTO `realms_status` VALUES (1391, 1, 'Dark', 1, 1, '2021-07-13 11:19:56', 0, 0);
INSERT INTO `realms_status` VALUES (1392, 1, 'Dark', 1, 1, '2021-07-13 11:20:11', 0, 0);
INSERT INTO `realms_status` VALUES (1393, 1, 'Dark', 1, 1, '2021-07-13 11:20:26', 0, 0);
INSERT INTO `realms_status` VALUES (1394, 1, 'Dark', 1, 1, '2021-07-13 11:20:42', 0, 0);
INSERT INTO `realms_status` VALUES (1395, 1, 'Dark', 1, 1, '2021-07-13 11:20:57', 0, 0);
INSERT INTO `realms_status` VALUES (1396, 1, 'Dark', 1, 1, '2021-07-13 11:21:11', 0, 0);
INSERT INTO `realms_status` VALUES (1397, 1, 'Dark', 1, 1, '2021-07-13 11:21:26', 0, 0);
INSERT INTO `realms_status` VALUES (1398, 1, 'Dark', 1, 1, '2021-07-13 11:21:41', 0, 0);
INSERT INTO `realms_status` VALUES (1399, 1, 'Dark', 1, 1, '2021-07-13 11:21:56', 0, 0);
INSERT INTO `realms_status` VALUES (1400, 1, 'Dark', 1, 1, '2021-07-13 11:22:11', 0, 0);
INSERT INTO `realms_status` VALUES (1401, 1, 'Dark', 1, 1, '2021-07-13 11:22:27', 0, 0);
INSERT INTO `realms_status` VALUES (1402, 1, 'Dark', 1, 1, '2021-07-13 11:22:41', 0, 0);
INSERT INTO `realms_status` VALUES (1403, 1, 'Dark', 1, 1, '2021-07-13 11:22:56', 0, 0);
INSERT INTO `realms_status` VALUES (1404, 1, 'Dark', 1, 1, '2021-07-13 11:23:11', 0, 0);
INSERT INTO `realms_status` VALUES (1405, 1, 'Dark', 1, 1, '2021-07-13 11:23:26', 0, 0);
INSERT INTO `realms_status` VALUES (1406, 1, 'Dark', 1, 1, '2021-07-13 11:23:41', 0, 0);
INSERT INTO `realms_status` VALUES (1407, 1, 'Dark', 1, 1, '2021-07-13 11:23:56', 0, 0);
INSERT INTO `realms_status` VALUES (1408, 1, 'Dark', 1, 1, '2021-07-13 11:25:20', 0, 0);
INSERT INTO `realms_status` VALUES (1409, 1, 'Dark', 1, 1, '2021-07-13 11:25:38', 0, 0);
INSERT INTO `realms_status` VALUES (1410, 1, 'Dark', 1, 1, '2021-07-13 11:25:49', 0, 0);
INSERT INTO `realms_status` VALUES (1411, 1, 'Dark', 1, 1, '2021-07-13 11:26:04', 0, 0);
INSERT INTO `realms_status` VALUES (1412, 1, 'Dark', 1, 1, '2021-07-13 11:26:19', 0, 0);
INSERT INTO `realms_status` VALUES (1413, 1, 'Dark', 1, 1, '2021-07-13 11:26:34', 0, 0);
INSERT INTO `realms_status` VALUES (1414, 1, 'Dark', 1, 1, '2021-07-13 11:26:50', 0, 0);
INSERT INTO `realms_status` VALUES (1415, 1, 'Dark', 1, 1, '2021-07-13 11:37:17', 0, 0);
INSERT INTO `realms_status` VALUES (1416, 1, 'Dark', 1, 1, '2021-07-13 11:37:32', 1, 1);
INSERT INTO `realms_status` VALUES (1417, 1, 'Dark', 1, 1, '2021-07-13 11:37:47', 1, 1);
INSERT INTO `realms_status` VALUES (1418, 1, 'Dark', 1, 1, '2021-07-13 11:39:08', 0, 0);
INSERT INTO `realms_status` VALUES (1419, 1, 'Dark', 1, 1, '2021-07-13 11:39:23', 1, 1);
INSERT INTO `realms_status` VALUES (1420, 1, 'Dark', 1, 1, '2021-07-13 11:39:38', 1, 1);
INSERT INTO `realms_status` VALUES (1421, 1, 'Dark', 1, 1, '2021-07-13 11:39:53', 1, 1);
INSERT INTO `realms_status` VALUES (1422, 1, 'Dark', 1, 1, '2021-07-13 11:40:08', 1, 1);
INSERT INTO `realms_status` VALUES (1423, 1, 'Dark', 1, 1, '2021-07-13 11:40:24', 1, 1);
INSERT INTO `realms_status` VALUES (1424, 1, 'Dark', 1, 1, '2021-07-13 11:40:39', 1, 1);
INSERT INTO `realms_status` VALUES (1425, 1, 'Dark', 1, 1, '2021-07-13 11:40:53', 1, 1);
INSERT INTO `realms_status` VALUES (1426, 1, 'Dark', 1, 1, '2021-07-13 11:41:08', 1, 1);
INSERT INTO `realms_status` VALUES (1427, 1, 'Dark', 1, 1, '2021-07-13 11:41:23', 1, 1);
INSERT INTO `realms_status` VALUES (1428, 1, 'Dark', 1, 1, '2021-07-13 11:41:38', 1, 1);
INSERT INTO `realms_status` VALUES (1429, 1, 'Dark', 1, 1, '2021-07-13 11:41:54', 1, 1);
INSERT INTO `realms_status` VALUES (1430, 1, 'Dark', 1, 1, '2021-07-13 11:42:09', 1, 1);
INSERT INTO `realms_status` VALUES (1431, 1, 'Dark', 1, 1, '2021-07-13 11:42:23', 1, 1);
INSERT INTO `realms_status` VALUES (1432, 1, 'Dark', 1, 1, '2021-07-13 11:42:38', 1, 1);
INSERT INTO `realms_status` VALUES (1433, 1, 'Dark', 1, 1, '2021-07-13 11:42:53', 1, 1);
INSERT INTO `realms_status` VALUES (1434, 1, 'Dark', 1, 1, '2021-07-13 11:43:08', 1, 1);
INSERT INTO `realms_status` VALUES (1435, 1, 'Dark', 1, 1, '2021-07-13 11:43:23', 1, 1);
INSERT INTO `realms_status` VALUES (1436, 1, 'Dark', 1, 1, '2021-07-13 11:43:39', 1, 1);
INSERT INTO `realms_status` VALUES (1437, 1, 'Dark', 1, 1, '2021-07-13 11:43:54', 1, 1);
INSERT INTO `realms_status` VALUES (1438, 1, 'Dark', 1, 1, '2021-07-13 11:44:09', 1, 1);
INSERT INTO `realms_status` VALUES (1439, 1, 'Dark', 1, 1, '2021-07-13 11:44:24', 1, 1);
INSERT INTO `realms_status` VALUES (1440, 1, 'Dark', 1, 1, '2021-07-13 11:44:39', 1, 1);
INSERT INTO `realms_status` VALUES (1441, 1, 'Dark', 1, 1, '2021-07-13 11:44:54', 1, 1);
INSERT INTO `realms_status` VALUES (1442, 1, 'Dark', 1, 1, '2021-07-13 11:45:10', 1, 1);
INSERT INTO `realms_status` VALUES (1443, 1, 'Dark', 1, 1, '2021-07-13 11:45:25', 1, 1);
INSERT INTO `realms_status` VALUES (1444, 1, 'Dark', 1, 1, '2021-07-13 11:45:39', 1, 1);
INSERT INTO `realms_status` VALUES (1445, 1, 'Dark', 1, 1, '2021-07-13 11:45:54', 1, 1);
INSERT INTO `realms_status` VALUES (1446, 1, 'Dark', 1, 1, '2021-07-13 11:46:09', 1, 1);
INSERT INTO `realms_status` VALUES (1447, 1, 'Dark', 1, 1, '2021-07-13 11:46:24', 1, 1);
INSERT INTO `realms_status` VALUES (1448, 1, 'Dark', 1, 1, '2021-07-13 11:46:39', 1, 1);
INSERT INTO `realms_status` VALUES (1449, 1, 'Dark', 1, 1, '2021-07-13 11:46:55', 1, 1);
INSERT INTO `realms_status` VALUES (1450, 1, 'Dark', 1, 1, '2021-07-13 11:47:10', 1, 1);
INSERT INTO `realms_status` VALUES (1451, 1, 'Dark', 1, 1, '2021-07-13 11:47:24', 1, 1);
INSERT INTO `realms_status` VALUES (1452, 1, 'Dark', 1, 1, '2021-07-13 11:47:39', 1, 1);
INSERT INTO `realms_status` VALUES (1453, 1, 'Dark', 1, 1, '2021-07-13 11:47:54', 1, 1);
INSERT INTO `realms_status` VALUES (1454, 1, 'Dark', 1, 1, '2021-07-13 11:48:09', 1, 1);
INSERT INTO `realms_status` VALUES (1455, 1, 'Dark', 1, 1, '2021-07-13 11:48:24', 1, 1);
INSERT INTO `realms_status` VALUES (1456, 1, 'Dark', 1, 1, '2021-07-13 11:48:40', 1, 1);
INSERT INTO `realms_status` VALUES (1457, 1, 'Dark', 1, 1, '2021-07-13 11:48:55', 1, 1);
INSERT INTO `realms_status` VALUES (1458, 1, 'Dark', 1, 1, '2021-07-13 11:49:09', 1, 1);
INSERT INTO `realms_status` VALUES (1459, 1, 'Dark', 1, 1, '2021-07-13 11:49:24', 1, 1);
INSERT INTO `realms_status` VALUES (1460, 1, 'Dark', 1, 1, '2021-07-13 11:49:39', 1, 1);
INSERT INTO `realms_status` VALUES (1461, 1, 'Dark', 1, 1, '2021-07-13 11:49:54', 1, 1);
INSERT INTO `realms_status` VALUES (1462, 1, 'Dark', 1, 1, '2021-07-13 11:50:09', 1, 1);
INSERT INTO `realms_status` VALUES (1463, 1, 'Dark', 1, 1, '2021-07-13 11:50:25', 1, 1);
INSERT INTO `realms_status` VALUES (1464, 1, 'Dark', 1, 1, '2021-07-13 11:50:40', 1, 1);
INSERT INTO `realms_status` VALUES (1465, 1, 'Dark', 1, 1, '2021-07-13 11:50:54', 1, 1);
INSERT INTO `realms_status` VALUES (1466, 1, 'Dark', 1, 1, '2021-07-13 11:51:09', 1, 1);
INSERT INTO `realms_status` VALUES (1467, 1, 'Dark', 1, 1, '2021-07-13 11:51:24', 1, 1);
INSERT INTO `realms_status` VALUES (1468, 1, 'Dark', 1, 1, '2021-07-13 11:51:39', 1, 1);
INSERT INTO `realms_status` VALUES (1469, 1, 'Dark', 1, 1, '2021-07-13 11:51:55', 1, 1);
INSERT INTO `realms_status` VALUES (1470, 1, 'Dark', 1, 1, '2021-07-13 11:52:10', 1, 1);
INSERT INTO `realms_status` VALUES (1471, 1, 'Dark', 1, 1, '2021-07-13 11:52:25', 1, 1);
INSERT INTO `realms_status` VALUES (1472, 1, 'Dark', 1, 1, '2021-07-13 11:52:39', 1, 1);
INSERT INTO `realms_status` VALUES (1473, 1, 'Dark', 1, 1, '2021-07-13 11:52:54', 1, 1);
INSERT INTO `realms_status` VALUES (1474, 1, 'Dark', 1, 1, '2021-07-13 11:53:09', 1, 1);
INSERT INTO `realms_status` VALUES (1475, 1, 'Dark', 1, 1, '2021-07-13 11:53:24', 1, 1);
INSERT INTO `realms_status` VALUES (1476, 1, 'Dark', 1, 1, '2021-07-13 11:53:39', 1, 1);
INSERT INTO `realms_status` VALUES (1477, 1, 'Dark', 1, 1, '2021-07-13 11:53:55', 1, 1);
INSERT INTO `realms_status` VALUES (1478, 1, 'Dark', 1, 1, '2021-07-13 11:54:10', 1, 1);
INSERT INTO `realms_status` VALUES (1479, 1, 'Dark', 1, 1, '2021-07-13 11:54:24', 1, 1);
INSERT INTO `realms_status` VALUES (1480, 1, 'Dark', 1, 1, '2021-07-13 11:54:39', 1, 1);
INSERT INTO `realms_status` VALUES (1481, 1, 'Dark', 1, 1, '2021-07-13 11:54:54', 1, 1);
INSERT INTO `realms_status` VALUES (1482, 1, 'Dark', 1, 1, '2021-07-13 11:55:09', 1, 1);
INSERT INTO `realms_status` VALUES (1483, 1, 'Dark', 1, 1, '2021-07-13 11:55:24', 1, 1);
INSERT INTO `realms_status` VALUES (1484, 1, 'Dark', 1, 1, '2021-07-13 11:55:40', 1, 1);
INSERT INTO `realms_status` VALUES (1485, 1, 'Dark', 1, 1, '2021-07-13 11:55:55', 1, 1);
INSERT INTO `realms_status` VALUES (1486, 1, 'Dark', 1, 1, '2021-07-13 11:56:09', 1, 1);
INSERT INTO `realms_status` VALUES (1487, 1, 'Dark', 1, 1, '2021-07-13 11:56:24', 1, 1);
INSERT INTO `realms_status` VALUES (1488, 1, 'Dark', 1, 1, '2021-07-13 11:56:39', 1, 1);
INSERT INTO `realms_status` VALUES (1489, 1, 'Dark', 1, 1, '2021-07-13 11:56:54', 1, 1);
INSERT INTO `realms_status` VALUES (1490, 1, 'Dark', 1, 1, '2021-07-13 11:57:09', 1, 1);
INSERT INTO `realms_status` VALUES (1491, 1, 'Dark', 1, 1, '2021-07-13 11:57:24', 1, 1);
INSERT INTO `realms_status` VALUES (1492, 1, 'Dark', 1, 1, '2021-07-13 11:57:40', 1, 1);
INSERT INTO `realms_status` VALUES (1493, 1, 'Dark', 1, 1, '2021-07-13 11:57:54', 1, 1);
INSERT INTO `realms_status` VALUES (1494, 1, 'Dark', 1, 1, '2021-07-13 11:58:09', 1, 1);
INSERT INTO `realms_status` VALUES (1495, 1, 'Dark', 1, 1, '2021-07-13 11:58:24', 1, 1);
INSERT INTO `realms_status` VALUES (1496, 1, 'Dark', 1, 1, '2021-07-13 11:58:39', 1, 1);
INSERT INTO `realms_status` VALUES (1497, 1, 'Dark', 1, 1, '2021-07-13 11:58:54', 1, 1);
INSERT INTO `realms_status` VALUES (1498, 1, 'Dark', 1, 1, '2021-07-13 11:59:10', 1, 1);
INSERT INTO `realms_status` VALUES (1499, 1, 'Dark', 1, 1, '2021-07-13 11:59:25', 1, 1);
INSERT INTO `realms_status` VALUES (1500, 1, 'Dark', 1, 1, '2021-07-13 11:59:39', 1, 1);
INSERT INTO `realms_status` VALUES (1501, 1, 'Dark', 1, 1, '2021-07-13 11:59:54', 1, 1);
INSERT INTO `realms_status` VALUES (1502, 1, 'Dark', 1, 1, '2021-07-13 12:00:09', 1, 1);
INSERT INTO `realms_status` VALUES (1503, 1, 'Dark', 1, 1, '2021-07-13 12:00:24', 1, 1);
INSERT INTO `realms_status` VALUES (1504, 1, 'Dark', 1, 1, '2021-07-13 12:00:40', 1, 1);
INSERT INTO `realms_status` VALUES (1505, 1, 'Dark', 1, 1, '2021-07-13 12:00:55', 1, 1);
INSERT INTO `realms_status` VALUES (1506, 1, 'Dark', 1, 1, '2021-07-13 12:01:10', 1, 1);
INSERT INTO `realms_status` VALUES (1507, 1, 'Dark', 1, 1, '2021-07-13 12:01:25', 1, 1);
INSERT INTO `realms_status` VALUES (1508, 1, 'Dark', 1, 1, '2021-07-13 12:01:40', 1, 1);
INSERT INTO `realms_status` VALUES (1509, 1, 'Dark', 1, 1, '2021-07-13 12:01:55', 1, 1);
INSERT INTO `realms_status` VALUES (1510, 1, 'Dark', 1, 1, '2021-07-13 12:02:10', 1, 1);
INSERT INTO `realms_status` VALUES (1511, 1, 'Dark', 1, 1, '2021-07-13 12:02:26', 1, 1);
INSERT INTO `realms_status` VALUES (1512, 1, 'Dark', 1, 1, '2021-07-13 12:02:41', 1, 1);
INSERT INTO `realms_status` VALUES (1513, 1, 'Dark', 1, 1, '2021-07-13 12:02:55', 1, 1);
INSERT INTO `realms_status` VALUES (1514, 1, 'Dark', 1, 1, '2021-07-13 12:03:10', 1, 1);
INSERT INTO `realms_status` VALUES (1515, 1, 'Dark', 1, 1, '2021-07-13 12:03:25', 1, 1);
INSERT INTO `realms_status` VALUES (1516, 1, 'Dark', 1, 1, '2021-07-13 12:03:40', 1, 1);
INSERT INTO `realms_status` VALUES (1517, 1, 'Dark', 1, 1, '2021-07-13 12:03:56', 1, 1);
INSERT INTO `realms_status` VALUES (1518, 1, 'Dark', 1, 1, '2021-07-13 12:04:11', 1, 1);
INSERT INTO `realms_status` VALUES (1519, 1, 'Dark', 1, 1, '2021-07-13 12:04:25', 1, 1);
INSERT INTO `realms_status` VALUES (1520, 1, 'Dark', 1, 1, '2021-07-13 12:04:40', 1, 1);
INSERT INTO `realms_status` VALUES (1521, 1, 'Dark', 1, 1, '2021-07-13 12:04:55', 1, 1);
INSERT INTO `realms_status` VALUES (1522, 1, 'Dark', 1, 1, '2021-07-13 12:05:10', 1, 1);
INSERT INTO `realms_status` VALUES (1523, 1, 'Dark', 1, 1, '2021-07-13 12:05:25', 1, 1);
INSERT INTO `realms_status` VALUES (1524, 1, 'Dark', 1, 1, '2021-07-13 12:05:41', 1, 1);
INSERT INTO `realms_status` VALUES (1525, 1, 'Dark', 1, 1, '2021-07-13 12:05:55', 1, 1);
INSERT INTO `realms_status` VALUES (1526, 1, 'Dark', 1, 1, '2021-07-13 12:06:10', 1, 1);
INSERT INTO `realms_status` VALUES (1527, 1, 'Dark', 1, 1, '2021-07-13 12:06:25', 1, 1);
INSERT INTO `realms_status` VALUES (1528, 1, 'Dark', 1, 1, '2021-07-13 12:06:40', 1, 1);
INSERT INTO `realms_status` VALUES (1529, 1, 'Dark', 1, 1, '2021-07-13 12:06:55', 1, 1);
INSERT INTO `realms_status` VALUES (1530, 1, 'Dark', 1, 1, '2021-07-13 12:07:11', 1, 1);
INSERT INTO `realms_status` VALUES (1531, 1, 'Dark', 1, 1, '2021-07-13 12:07:25', 1, 1);
INSERT INTO `realms_status` VALUES (1532, 1, 'Dark', 1, 1, '2021-07-13 12:07:40', 1, 1);
INSERT INTO `realms_status` VALUES (1533, 1, 'Dark', 1, 1, '2021-07-13 12:07:55', 1, 1);
INSERT INTO `realms_status` VALUES (1534, 1, 'Dark', 1, 1, '2021-07-13 12:08:10', 1, 1);
INSERT INTO `realms_status` VALUES (1535, 1, 'Dark', 1, 1, '2021-07-13 12:08:25', 1, 1);
INSERT INTO `realms_status` VALUES (1536, 1, 'Dark', 1, 1, '2021-07-13 12:08:41', 1, 1);
INSERT INTO `realms_status` VALUES (1537, 1, 'Dark', 1, 1, '2021-07-13 12:08:56', 1, 1);
INSERT INTO `realms_status` VALUES (1538, 1, 'Dark', 1, 1, '2021-07-13 12:09:10', 1, 1);
INSERT INTO `realms_status` VALUES (1539, 1, 'Dark', 1, 1, '2021-07-13 12:09:25', 1, 1);
INSERT INTO `realms_status` VALUES (1540, 1, 'Dark', 1, 1, '2021-07-13 12:09:40', 1, 1);
INSERT INTO `realms_status` VALUES (1541, 1, 'Dark', 1, 1, '2021-07-13 12:09:55', 1, 1);
INSERT INTO `realms_status` VALUES (1542, 1, 'Dark', 1, 1, '2021-07-13 12:10:10', 1, 1);
INSERT INTO `realms_status` VALUES (1543, 1, 'Dark', 1, 1, '2021-07-13 12:10:26', 1, 1);
INSERT INTO `realms_status` VALUES (1544, 1, 'Dark', 1, 1, '2021-07-13 12:10:40', 1, 1);
INSERT INTO `realms_status` VALUES (1545, 1, 'Dark', 1, 1, '2021-07-13 12:10:55', 1, 1);
INSERT INTO `realms_status` VALUES (1546, 1, 'Dark', 1, 1, '2021-07-13 12:11:10', 1, 1);
INSERT INTO `realms_status` VALUES (1547, 1, 'Dark', 1, 1, '2021-07-13 12:11:25', 1, 1);
INSERT INTO `realms_status` VALUES (1548, 1, 'Dark', 1, 1, '2021-07-13 12:11:40', 1, 1);
INSERT INTO `realms_status` VALUES (1549, 1, 'Dark', 1, 1, '2021-07-13 12:11:56', 1, 1);
INSERT INTO `realms_status` VALUES (1550, 1, 'Dark', 1, 1, '2021-07-13 12:12:10', 1, 1);
INSERT INTO `realms_status` VALUES (1551, 1, 'Dark', 1, 1, '2021-07-13 12:12:25', 1, 1);
INSERT INTO `realms_status` VALUES (1552, 1, 'Dark', 1, 1, '2021-07-13 12:12:40', 1, 1);
INSERT INTO `realms_status` VALUES (1553, 1, 'Dark', 1, 1, '2021-07-13 12:12:55', 1, 1);
INSERT INTO `realms_status` VALUES (1554, 1, 'Dark', 1, 1, '2021-07-13 12:13:10', 1, 1);
INSERT INTO `realms_status` VALUES (1555, 1, 'Dark', 1, 1, '2021-07-13 12:13:25', 1, 1);
INSERT INTO `realms_status` VALUES (1556, 1, 'Dark', 1, 1, '2021-07-13 12:13:41', 1, 1);
INSERT INTO `realms_status` VALUES (1557, 1, 'Dark', 1, 1, '2021-07-13 12:13:55', 1, 1);
INSERT INTO `realms_status` VALUES (1558, 1, 'Dark', 1, 1, '2021-07-13 12:14:10', 1, 1);
INSERT INTO `realms_status` VALUES (1559, 1, 'Dark', 1, 1, '2021-07-13 12:14:25', 1, 1);
INSERT INTO `realms_status` VALUES (1560, 1, 'Dark', 1, 1, '2021-07-13 12:14:40', 1, 1);
INSERT INTO `realms_status` VALUES (1561, 1, 'Dark', 1, 1, '2021-07-13 12:14:55', 1, 1);
INSERT INTO `realms_status` VALUES (1562, 1, 'Dark', 1, 1, '2021-07-13 12:15:11', 1, 1);
INSERT INTO `realms_status` VALUES (1563, 1, 'Dark', 1, 1, '2021-07-13 12:15:26', 1, 1);
INSERT INTO `realms_status` VALUES (1564, 1, 'Dark', 1, 1, '2021-07-13 12:15:40', 1, 1);
INSERT INTO `realms_status` VALUES (1565, 1, 'Dark', 1, 1, '2021-07-13 12:15:55', 1, 1);
INSERT INTO `realms_status` VALUES (1566, 1, 'Dark', 1, 1, '2021-07-13 12:16:10', 1, 1);
INSERT INTO `realms_status` VALUES (1567, 1, 'Dark', 1, 1, '2021-07-13 12:16:25', 1, 1);
INSERT INTO `realms_status` VALUES (1568, 1, 'Dark', 1, 1, '2021-07-13 12:16:41', 1, 1);
INSERT INTO `realms_status` VALUES (1569, 1, 'Dark', 1, 1, '2021-07-13 12:16:56', 1, 1);
INSERT INTO `realms_status` VALUES (1570, 1, 'Dark', 1, 1, '2021-07-13 12:17:10', 1, 1);
INSERT INTO `realms_status` VALUES (1571, 1, 'Dark', 1, 1, '2021-07-13 12:17:25', 1, 1);
INSERT INTO `realms_status` VALUES (1572, 1, 'Dark', 1, 1, '2021-07-13 12:17:40', 1, 1);
INSERT INTO `realms_status` VALUES (1573, 1, 'Dark', 1, 1, '2021-07-13 12:17:55', 1, 1);
INSERT INTO `realms_status` VALUES (1574, 1, 'Dark', 1, 1, '2021-07-13 12:18:10', 1, 1);
INSERT INTO `realms_status` VALUES (1575, 1, 'Dark', 1, 1, '2021-07-13 12:18:26', 1, 1);
INSERT INTO `realms_status` VALUES (1576, 1, 'Dark', 1, 1, '2021-07-13 12:18:41', 1, 1);
INSERT INTO `realms_status` VALUES (1577, 1, 'Dark', 1, 1, '2021-07-13 12:18:56', 1, 1);
INSERT INTO `realms_status` VALUES (1578, 1, 'Dark', 1, 1, '2021-07-13 12:19:11', 1, 1);
INSERT INTO `realms_status` VALUES (1579, 1, 'Dark', 1, 1, '2021-07-13 12:19:26', 1, 1);
INSERT INTO `realms_status` VALUES (1580, 1, 'Dark', 1, 1, '2021-07-13 12:19:41', 1, 1);
INSERT INTO `realms_status` VALUES (1581, 1, 'Dark', 1, 1, '2021-07-13 12:19:56', 1, 1);
INSERT INTO `realms_status` VALUES (1582, 1, 'Dark', 1, 1, '2021-07-13 12:20:12', 1, 1);
INSERT INTO `realms_status` VALUES (1583, 1, 'Dark', 1, 1, '2021-07-13 12:20:26', 1, 1);
INSERT INTO `realms_status` VALUES (1584, 1, 'Dark', 1, 1, '2021-07-13 12:20:41', 1, 1);
INSERT INTO `realms_status` VALUES (1585, 1, 'Dark', 1, 1, '2021-07-13 12:20:56', 1, 1);
INSERT INTO `realms_status` VALUES (1586, 1, 'Dark', 1, 1, '2021-07-13 12:21:11', 1, 1);
INSERT INTO `realms_status` VALUES (1587, 1, 'Dark', 1, 1, '2021-07-13 12:21:26', 1, 1);
INSERT INTO `realms_status` VALUES (1588, 1, 'Dark', 1, 1, '2021-07-13 14:14:55', 0, 0);
INSERT INTO `realms_status` VALUES (1589, 1, 'Dark', 1, 1, '2021-07-13 14:16:58', 0, 1);
INSERT INTO `realms_status` VALUES (1590, 1, 'Dark', 1, 1, '2021-07-13 14:17:13', 0, 1);
INSERT INTO `realms_status` VALUES (1591, 1, 'Dark', 1, 1, '2021-07-13 14:20:36', 0, 0);
INSERT INTO `realms_status` VALUES (1592, 1, 'Dark', 1, 1, '2021-07-13 14:20:51', 1, 1);
INSERT INTO `realms_status` VALUES (1593, 1, 'Dark', 1, 1, '2021-07-13 14:21:21', 0, 0);
INSERT INTO `realms_status` VALUES (1594, 1, 'Dark', 1, 1, '2021-07-13 14:21:36', 1, 1);
INSERT INTO `realms_status` VALUES (1595, 1, 'Dark', 1, 1, '2021-07-13 14:21:52', 1, 1);
INSERT INTO `realms_status` VALUES (1596, 1, 'Dark', 1, 1, '2021-07-13 14:22:06', 1, 1);
INSERT INTO `realms_status` VALUES (1597, 1, 'Dark', 1, 1, '2021-07-13 14:22:21', 1, 1);
INSERT INTO `realms_status` VALUES (1598, 1, 'Dark', 1, 1, '2021-07-13 14:22:36', 1, 1);
INSERT INTO `realms_status` VALUES (1599, 1, 'Dark', 1, 1, '2021-07-13 14:22:51', 1, 1);
INSERT INTO `realms_status` VALUES (1600, 1, 'Dark', 1, 1, '2021-07-13 14:23:06', 1, 1);
INSERT INTO `realms_status` VALUES (1601, 1, 'Dark', 1, 1, '2021-07-13 14:23:22', 1, 1);
INSERT INTO `realms_status` VALUES (1602, 1, 'Dark', 1, 1, '2021-07-13 14:23:37', 1, 1);
INSERT INTO `realms_status` VALUES (1603, 1, 'Dark', 1, 1, '2021-07-13 14:23:51', 1, 1);
INSERT INTO `realms_status` VALUES (1604, 1, 'Dark', 1, 1, '2021-07-13 14:24:06', 1, 1);
INSERT INTO `realms_status` VALUES (1605, 1, 'Dark', 1, 1, '2021-07-13 14:24:21', 1, 1);
INSERT INTO `realms_status` VALUES (1606, 1, 'Dark', 1, 1, '2021-07-13 14:24:36', 1, 1);
INSERT INTO `realms_status` VALUES (1607, 1, 'Dark', 1, 1, '2021-07-13 14:24:51', 1, 1);
INSERT INTO `realms_status` VALUES (1608, 1, 'Dark', 1, 1, '2021-07-13 14:25:06', 1, 1);
INSERT INTO `realms_status` VALUES (1609, 1, 'Dark', 1, 1, '2021-07-13 14:25:22', 1, 1);
INSERT INTO `realms_status` VALUES (1610, 1, 'Dark', 1, 1, '2021-07-13 14:25:36', 1, 1);
INSERT INTO `realms_status` VALUES (1611, 1, 'Dark', 1, 1, '2021-07-13 14:25:51', 1, 1);
INSERT INTO `realms_status` VALUES (1612, 1, 'Dark', 1, 1, '2021-07-13 14:26:06', 1, 1);
INSERT INTO `realms_status` VALUES (1613, 1, 'Dark', 1, 1, '2021-07-13 14:27:02', 0, 0);
INSERT INTO `realms_status` VALUES (1614, 1, 'Dark', 1, 1, '2021-07-13 14:30:44', 0, 0);
INSERT INTO `realms_status` VALUES (1615, 1, 'Dark', 1, 1, '2021-07-13 14:31:00', 1, 1);
INSERT INTO `realms_status` VALUES (1616, 1, 'Dark', 1, 1, '2021-07-13 14:31:14', 1, 1);
INSERT INTO `realms_status` VALUES (1617, 1, 'Dark', 1, 1, '2021-07-13 14:31:29', 1, 1);
INSERT INTO `realms_status` VALUES (1618, 1, 'Dark', 1, 1, '2021-07-13 14:31:44', 1, 1);
INSERT INTO `realms_status` VALUES (1619, 1, 'Dark', 1, 1, '2021-07-13 14:31:59', 1, 1);
INSERT INTO `realms_status` VALUES (1620, 1, 'Dark', 1, 1, '2021-07-13 14:32:14', 1, 1);
INSERT INTO `realms_status` VALUES (1621, 1, 'Dark', 1, 1, '2021-07-13 14:32:30', 1, 1);
INSERT INTO `realms_status` VALUES (1622, 1, 'Dark', 1, 1, '2021-07-13 14:32:44', 1, 1);
INSERT INTO `realms_status` VALUES (1623, 1, 'Dark', 1, 1, '2021-07-13 14:32:59', 1, 1);
INSERT INTO `realms_status` VALUES (1624, 1, 'Dark', 1, 1, '2021-07-13 14:33:14', 1, 1);
INSERT INTO `realms_status` VALUES (1625, 1, 'Dark', 1, 1, '2021-07-13 14:33:29', 0, 1);
INSERT INTO `realms_status` VALUES (1626, 1, 'Dark', 1, 1, '2021-07-13 14:33:44', 0, 1);
INSERT INTO `realms_status` VALUES (1627, 1, 'Dark', 1, 1, '2021-07-13 14:34:00', 0, 1);
INSERT INTO `realms_status` VALUES (1628, 1, 'Dark', 1, 1, '2021-07-13 14:34:15', 0, 1);
INSERT INTO `realms_status` VALUES (1629, 1, 'Dark', 1, 1, '2021-07-13 14:34:29', 1, 1);
INSERT INTO `realms_status` VALUES (1630, 1, 'Dark', 1, 1, '2021-07-13 14:34:44', 1, 1);
INSERT INTO `realms_status` VALUES (1631, 1, 'Dark', 1, 1, '2021-07-13 14:34:59', 1, 1);
INSERT INTO `realms_status` VALUES (1632, 1, 'Dark', 1, 1, '2021-07-13 14:35:14', 1, 1);
INSERT INTO `realms_status` VALUES (1633, 1, 'Dark', 1, 1, '2021-07-13 14:35:29', 1, 1);
INSERT INTO `realms_status` VALUES (1634, 1, 'Dark', 1, 1, '2021-07-13 14:35:45', 1, 1);
INSERT INTO `realms_status` VALUES (1635, 1, 'Dark', 1, 1, '2021-07-13 14:36:00', 1, 1);
INSERT INTO `realms_status` VALUES (1636, 1, 'Dark', 1, 1, '2021-07-13 14:36:14', 1, 1);
INSERT INTO `realms_status` VALUES (1637, 1, 'Dark', 1, 1, '2021-07-13 14:36:29', 1, 1);
INSERT INTO `realms_status` VALUES (1638, 1, 'Dark', 1, 1, '2021-07-13 14:36:44', 1, 1);
INSERT INTO `realms_status` VALUES (1639, 1, 'Dark', 1, 1, '2021-07-13 14:36:59', 1, 1);
INSERT INTO `realms_status` VALUES (1640, 1, 'Dark', 1, 1, '2021-07-13 14:37:14', 1, 1);
INSERT INTO `realms_status` VALUES (1641, 1, 'Dark', 1, 1, '2021-07-13 14:37:30', 1, 1);
INSERT INTO `realms_status` VALUES (1642, 1, 'Dark', 1, 1, '2021-07-13 14:37:45', 1, 1);
INSERT INTO `realms_status` VALUES (1643, 1, 'Dark', 1, 1, '2021-07-13 14:38:00', 0, 1);
INSERT INTO `realms_status` VALUES (1644, 1, 'Dark', 1, 1, '2021-07-13 14:38:15', 0, 1);
INSERT INTO `realms_status` VALUES (1645, 1, 'Dark', 1, 1, '2021-07-13 14:38:30', 0, 1);
INSERT INTO `realms_status` VALUES (1646, 1, 'Dark', 1, 1, '2021-07-13 14:38:45', 1, 1);
INSERT INTO `realms_status` VALUES (1647, 1, 'Dark', 1, 1, '2021-07-13 14:39:01', 1, 1);
INSERT INTO `realms_status` VALUES (1648, 1, 'Dark', 1, 1, '2021-07-13 14:39:16', 1, 1);
INSERT INTO `realms_status` VALUES (1649, 1, 'Dark', 1, 1, '2021-07-13 14:39:30', 1, 1);
INSERT INTO `realms_status` VALUES (1650, 1, 'Dark', 1, 1, '2021-07-13 14:39:45', 1, 1);
INSERT INTO `realms_status` VALUES (1651, 1, 'Dark', 1, 1, '2021-07-13 14:40:00', 1, 1);
INSERT INTO `realms_status` VALUES (1652, 1, 'Dark', 1, 1, '2021-07-13 14:40:15', 1, 1);
INSERT INTO `realms_status` VALUES (1653, 1, 'Dark', 1, 1, '2021-07-13 14:40:31', 1, 1);
INSERT INTO `realms_status` VALUES (1654, 1, 'Dark', 1, 1, '2021-07-13 14:40:46', 1, 1);
INSERT INTO `realms_status` VALUES (1655, 1, 'Dark', 1, 1, '2021-07-13 14:41:00', 1, 1);
INSERT INTO `realms_status` VALUES (1656, 1, 'Dark', 1, 1, '2021-07-13 14:41:15', 1, 1);
INSERT INTO `realms_status` VALUES (1657, 1, 'Dark', 1, 1, '2021-07-13 14:41:30', 0, 1);
INSERT INTO `realms_status` VALUES (1658, 1, 'Dark', 1, 1, '2021-07-13 14:41:45', 0, 1);
INSERT INTO `realms_status` VALUES (1659, 1, 'Dark', 1, 1, '2021-07-13 14:42:00', 0, 1);
INSERT INTO `realms_status` VALUES (1660, 1, 'Dark', 1, 1, '2021-07-13 14:42:16', 0, 1);
INSERT INTO `realms_status` VALUES (1661, 1, 'Dark', 1, 1, '2021-07-13 14:42:31', 0, 1);
INSERT INTO `realms_status` VALUES (1662, 1, 'Dark', 1, 1, '2021-07-13 14:42:45', 0, 1);
INSERT INTO `realms_status` VALUES (1663, 1, 'Dark', 1, 1, '2021-07-13 14:43:00', 0, 1);
INSERT INTO `realms_status` VALUES (1664, 1, 'Dark', 1, 1, '2021-07-13 14:43:15', 0, 1);
INSERT INTO `realms_status` VALUES (1665, 1, 'Dark', 1, 1, '2021-07-13 14:43:30', 0, 1);
INSERT INTO `realms_status` VALUES (1666, 1, 'Dark', 1, 1, '2021-07-13 14:43:45', 0, 1);
INSERT INTO `realms_status` VALUES (1667, 1, 'Dark', 1, 1, '2021-07-13 14:44:01', 0, 1);
INSERT INTO `realms_status` VALUES (1668, 1, 'Dark', 1, 1, '2021-07-13 14:44:15', 0, 1);
INSERT INTO `realms_status` VALUES (1669, 1, 'Dark', 1, 1, '2021-07-13 14:44:30', 0, 1);
INSERT INTO `realms_status` VALUES (1670, 1, 'Dark', 1, 1, '2021-07-13 14:44:45', 0, 1);
INSERT INTO `realms_status` VALUES (1671, 1, 'Dark', 1, 1, '2021-07-13 14:45:00', 0, 1);
INSERT INTO `realms_status` VALUES (1672, 1, 'Dark', 1, 1, '2021-07-13 14:45:15', 0, 1);
INSERT INTO `realms_status` VALUES (1673, 1, 'Dark', 1, 1, '2021-07-13 14:45:30', 0, 1);
INSERT INTO `realms_status` VALUES (1674, 1, 'Dark', 1, 1, '2021-07-13 14:45:46', 0, 1);
INSERT INTO `realms_status` VALUES (1675, 1, 'Dark', 1, 1, '2021-07-13 14:46:00', 0, 1);
INSERT INTO `realms_status` VALUES (1676, 1, 'Dark', 1, 1, '2021-07-13 14:46:15', 0, 1);
INSERT INTO `realms_status` VALUES (1677, 1, 'Dark', 1, 1, '2021-07-13 14:46:30', 0, 1);
INSERT INTO `realms_status` VALUES (1678, 1, 'Dark', 1, 1, '2021-07-13 14:46:45', 0, 1);
INSERT INTO `realms_status` VALUES (1679, 1, 'Dark', 1, 1, '2021-07-13 14:47:00', 0, 1);
INSERT INTO `realms_status` VALUES (1680, 1, 'Dark', 1, 1, '2021-07-13 14:47:16', 0, 1);
INSERT INTO `realms_status` VALUES (1681, 1, 'Dark', 1, 1, '2021-07-13 14:47:30', 0, 1);
INSERT INTO `realms_status` VALUES (1682, 1, 'Dark', 1, 1, '2021-07-13 14:47:45', 0, 1);
INSERT INTO `realms_status` VALUES (1683, 1, 'Dark', 1, 1, '2021-07-13 14:48:00', 0, 1);
INSERT INTO `realms_status` VALUES (1684, 1, 'Dark', 1, 1, '2021-07-13 14:48:15', 0, 1);
INSERT INTO `realms_status` VALUES (1685, 1, 'Dark', 1, 1, '2021-07-13 14:48:30', 0, 1);
INSERT INTO `realms_status` VALUES (1686, 1, 'Dark', 1, 1, '2021-07-13 14:48:45', 0, 1);
INSERT INTO `realms_status` VALUES (1687, 1, 'Dark', 1, 1, '2021-07-13 14:49:01', 0, 1);
INSERT INTO `realms_status` VALUES (1688, 1, 'Dark', 1, 1, '2021-07-13 14:49:15', 0, 1);
INSERT INTO `realms_status` VALUES (1689, 1, 'Dark', 1, 1, '2021-07-13 14:49:30', 0, 1);
INSERT INTO `realms_status` VALUES (1690, 1, 'Dark', 1, 1, '2021-07-13 14:49:45', 0, 1);
INSERT INTO `realms_status` VALUES (1691, 1, 'Dark', 1, 1, '2021-07-13 14:50:00', 0, 1);
INSERT INTO `realms_status` VALUES (1692, 1, 'Dark', 1, 1, '2021-07-13 14:50:15', 0, 1);
INSERT INTO `realms_status` VALUES (1693, 1, 'Dark', 1, 1, '2021-07-13 14:50:31', 0, 1);
INSERT INTO `realms_status` VALUES (1694, 1, 'Dark', 1, 1, '2021-07-13 14:50:46', 0, 1);
INSERT INTO `realms_status` VALUES (1695, 1, 'Dark', 1, 1, '2021-07-13 14:51:00', 0, 1);
INSERT INTO `realms_status` VALUES (1696, 1, 'Dark', 1, 1, '2021-07-13 14:51:15', 0, 1);
INSERT INTO `realms_status` VALUES (1697, 1, 'Dark', 1, 1, '2021-07-13 14:51:30', 0, 1);
INSERT INTO `realms_status` VALUES (1698, 1, 'Dark', 1, 1, '2021-07-13 14:51:45', 0, 1);
INSERT INTO `realms_status` VALUES (1699, 1, 'Dark', 1, 1, '2021-07-13 14:52:00', 0, 1);
INSERT INTO `realms_status` VALUES (1700, 1, 'Dark', 1, 1, '2021-07-13 14:52:16', 0, 1);
INSERT INTO `realms_status` VALUES (1701, 1, 'Dark', 1, 1, '2021-07-13 14:52:31', 0, 1);
INSERT INTO `realms_status` VALUES (1702, 1, 'Dark', 1, 1, '2021-07-13 14:52:45', 0, 1);
INSERT INTO `realms_status` VALUES (1703, 1, 'Dark', 1, 1, '2021-07-13 14:53:00', 0, 1);
INSERT INTO `realms_status` VALUES (1704, 1, 'Dark', 1, 1, '2021-07-13 14:53:15', 0, 1);
INSERT INTO `realms_status` VALUES (1705, 1, 'Dark', 1, 1, '2021-07-13 14:53:30', 0, 1);
INSERT INTO `realms_status` VALUES (1706, 1, 'Dark', 1, 1, '2021-07-13 14:53:45', 0, 1);
INSERT INTO `realms_status` VALUES (1707, 1, 'Dark', 1, 1, '2021-07-13 14:54:01', 0, 1);
INSERT INTO `realms_status` VALUES (1708, 1, 'Dark', 1, 1, '2021-07-13 14:54:15', 0, 1);
INSERT INTO `realms_status` VALUES (1709, 1, 'Dark', 1, 1, '2021-07-13 14:54:30', 1, 1);
INSERT INTO `realms_status` VALUES (1710, 1, 'Dark', 1, 1, '2021-07-13 14:54:45', 1, 1);
INSERT INTO `realms_status` VALUES (1711, 1, 'Dark', 1, 1, '2021-07-13 14:55:00', 0, 1);
INSERT INTO `realms_status` VALUES (1712, 1, 'Dark', 1, 1, '2021-07-13 14:55:15', 0, 1);
INSERT INTO `realms_status` VALUES (1713, 1, 'Dark', 1, 1, '2021-07-13 14:55:31', 0, 1);
INSERT INTO `realms_status` VALUES (1714, 1, 'Dark', 1, 1, '2021-07-13 14:55:46', 0, 1);
INSERT INTO `realms_status` VALUES (1715, 1, 'Dark', 1, 1, '2021-07-13 14:56:00', 0, 1);
INSERT INTO `realms_status` VALUES (1716, 1, 'Dark', 1, 1, '2021-07-13 14:56:15', 0, 1);
INSERT INTO `realms_status` VALUES (1717, 1, 'Dark', 1, 1, '2021-07-13 14:56:30', 0, 1);
INSERT INTO `realms_status` VALUES (1718, 1, 'Dark', 1, 1, '2021-07-13 14:56:45', 0, 1);
INSERT INTO `realms_status` VALUES (1719, 1, 'Dark', 1, 1, '2021-07-13 14:57:00', 0, 1);
INSERT INTO `realms_status` VALUES (1720, 1, 'Dark', 1, 1, '2021-07-13 14:57:16', 0, 1);
INSERT INTO `realms_status` VALUES (1721, 1, 'Dark', 1, 1, '2021-07-13 14:57:31', 0, 1);
INSERT INTO `realms_status` VALUES (1722, 1, 'Dark', 1, 1, '2021-07-13 14:57:45', 0, 1);
INSERT INTO `realms_status` VALUES (1723, 1, 'Dark', 1, 1, '2021-07-13 14:58:00', 0, 1);
INSERT INTO `realms_status` VALUES (1724, 1, 'Dark', 1, 1, '2021-07-13 14:58:15', 0, 1);
INSERT INTO `realms_status` VALUES (1725, 1, 'Dark', 1, 1, '2021-07-13 14:58:30', 0, 1);
INSERT INTO `realms_status` VALUES (1726, 1, 'Dark', 1, 1, '2021-07-13 14:58:45', 0, 1);
INSERT INTO `realms_status` VALUES (1727, 1, 'Dark', 1, 1, '2021-07-13 14:59:01', 0, 1);
INSERT INTO `realms_status` VALUES (1728, 1, 'Dark', 1, 1, '2021-07-13 14:59:16', 0, 1);
INSERT INTO `realms_status` VALUES (1729, 1, 'Dark', 1, 1, '2021-07-13 14:59:30', 0, 1);
INSERT INTO `realms_status` VALUES (1730, 1, 'Dark', 1, 1, '2021-07-13 14:59:45', 0, 1);
INSERT INTO `realms_status` VALUES (1731, 1, 'Dark', 1, 1, '2021-07-13 15:00:00', 0, 1);
INSERT INTO `realms_status` VALUES (1732, 1, 'Dark', 1, 1, '2021-07-13 15:00:15', 0, 1);
INSERT INTO `realms_status` VALUES (1733, 1, 'Dark', 1, 1, '2021-07-13 15:00:30', 0, 1);
INSERT INTO `realms_status` VALUES (1734, 1, 'Dark', 1, 1, '2021-07-13 15:00:46', 0, 1);
INSERT INTO `realms_status` VALUES (1735, 1, 'Dark', 1, 1, '2021-07-13 15:01:01', 0, 1);
INSERT INTO `realms_status` VALUES (1736, 1, 'Dark', 1, 1, '2021-07-13 15:01:16', 0, 1);
INSERT INTO `realms_status` VALUES (1737, 1, 'Dark', 1, 1, '2021-07-13 15:01:31', 0, 1);
INSERT INTO `realms_status` VALUES (1738, 1, 'Dark', 1, 1, '2021-07-13 15:01:46', 0, 1);
INSERT INTO `realms_status` VALUES (1739, 1, 'Dark', 1, 1, '2021-07-13 15:02:01', 0, 1);
INSERT INTO `realms_status` VALUES (1740, 1, 'Dark', 1, 1, '2021-07-13 15:02:16', 0, 1);
INSERT INTO `realms_status` VALUES (1741, 1, 'Dark', 1, 1, '2021-07-13 15:02:32', 0, 1);
INSERT INTO `realms_status` VALUES (1742, 1, 'Dark', 1, 1, '2021-07-13 15:02:47', 0, 1);
INSERT INTO `realms_status` VALUES (1743, 1, 'Dark', 1, 1, '2021-07-13 15:03:01', 1, 1);
INSERT INTO `realms_status` VALUES (1744, 1, 'Dark', 1, 1, '2021-07-13 15:03:16', 1, 1);
INSERT INTO `realms_status` VALUES (1745, 1, 'Dark', 1, 1, '2021-07-13 15:03:31', 1, 1);
INSERT INTO `realms_status` VALUES (1746, 1, 'Dark', 1, 1, '2021-07-13 15:03:46', 1, 1);
INSERT INTO `realms_status` VALUES (1747, 1, 'Dark', 1, 1, '2021-07-13 15:04:01', 1, 1);
INSERT INTO `realms_status` VALUES (1748, 1, 'Dark', 1, 1, '2021-07-13 15:04:17', 1, 1);
INSERT INTO `realms_status` VALUES (1749, 1, 'Dark', 1, 1, '2021-07-13 15:04:32', 1, 1);
INSERT INTO `realms_status` VALUES (1750, 1, 'Dark', 1, 1, '2021-07-13 15:04:47', 1, 1);
INSERT INTO `realms_status` VALUES (1751, 1, 'Dark', 1, 1, '2021-07-13 15:05:02', 1, 1);
INSERT INTO `realms_status` VALUES (1752, 1, 'Dark', 1, 1, '2021-07-13 15:05:17', 1, 1);
INSERT INTO `realms_status` VALUES (1753, 1, 'Dark', 1, 1, '2021-07-13 15:05:32', 1, 1);
INSERT INTO `realms_status` VALUES (1754, 1, 'Dark', 1, 1, '2021-07-13 15:05:47', 1, 1);
INSERT INTO `realms_status` VALUES (1755, 1, 'Dark', 1, 1, '2021-07-13 15:06:03', 1, 1);
INSERT INTO `realms_status` VALUES (1756, 1, 'Dark', 1, 1, '2021-07-13 15:06:18', 1, 1);
INSERT INTO `realms_status` VALUES (1757, 1, 'Dark', 1, 1, '2021-07-13 15:06:32', 1, 1);
INSERT INTO `realms_status` VALUES (1758, 1, 'Dark', 1, 1, '2021-07-13 15:06:47', 1, 1);
INSERT INTO `realms_status` VALUES (1759, 1, 'Dark', 1, 1, '2021-07-13 15:07:02', 1, 1);
INSERT INTO `realms_status` VALUES (1760, 1, 'Dark', 1, 1, '2021-07-13 15:07:17', 1, 1);
INSERT INTO `realms_status` VALUES (1761, 1, 'Dark', 1, 1, '2021-07-13 15:07:43', 1, 1);
INSERT INTO `realms_status` VALUES (1762, 1, 'Dark', 1, 1, '2021-07-13 15:07:55', 1, 1);
INSERT INTO `realms_status` VALUES (1763, 1, 'Dark', 1, 1, '2021-07-13 15:08:21', 1, 1);
INSERT INTO `realms_status` VALUES (1764, 1, 'Dark', 1, 1, '2021-07-13 15:09:04', 0, 0);
INSERT INTO `realms_status` VALUES (1765, 1, 'Dark', 1, 1, '2021-07-13 15:09:19', 0, 0);
INSERT INTO `realms_status` VALUES (1766, 1, 'Dark', 1, 1, '2021-07-13 15:09:34', 1, 1);
INSERT INTO `realms_status` VALUES (1767, 1, 'Dark', 1, 1, '2021-07-13 15:09:49', 1, 1);
INSERT INTO `realms_status` VALUES (1768, 1, 'Dark', 1, 1, '2021-07-13 15:10:04', 1, 1);
INSERT INTO `realms_status` VALUES (1769, 1, 'Dark', 1, 1, '2021-07-13 15:10:20', 1, 1);
INSERT INTO `realms_status` VALUES (1770, 1, 'Dark', 1, 1, '2021-07-13 15:10:34', 1, 1);
INSERT INTO `realms_status` VALUES (1771, 1, 'Dark', 1, 1, '2021-07-13 15:10:49', 1, 1);
INSERT INTO `realms_status` VALUES (1772, 1, 'Dark', 1, 1, '2021-07-13 15:11:04', 1, 1);
INSERT INTO `realms_status` VALUES (1773, 1, 'Dark', 1, 1, '2021-07-13 15:11:19', 1, 1);
INSERT INTO `realms_status` VALUES (1774, 1, 'Dark', 1, 1, '2021-07-13 15:11:34', 1, 1);
INSERT INTO `realms_status` VALUES (1775, 1, 'Dark', 1, 1, '2021-07-13 15:11:49', 1, 1);
INSERT INTO `realms_status` VALUES (1776, 1, 'Dark', 1, 1, '2021-07-13 15:12:05', 1, 1);
INSERT INTO `realms_status` VALUES (1777, 1, 'Dark', 1, 1, '2021-07-13 15:12:20', 1, 1);
INSERT INTO `realms_status` VALUES (1778, 1, 'Dark', 1, 1, '2021-07-13 15:12:34', 1, 1);
INSERT INTO `realms_status` VALUES (1779, 1, 'Dark', 1, 1, '2021-07-13 15:12:49', 1, 1);
INSERT INTO `realms_status` VALUES (1780, 1, 'Dark', 1, 1, '2021-07-13 15:13:04', 1, 1);
INSERT INTO `realms_status` VALUES (1781, 1, 'Dark', 1, 1, '2021-07-13 15:13:19', 1, 1);
INSERT INTO `realms_status` VALUES (1782, 1, 'Dark', 1, 1, '2021-07-13 15:13:34', 1, 1);
INSERT INTO `realms_status` VALUES (1783, 1, 'Dark', 1, 1, '2021-07-13 15:13:50', 1, 1);
INSERT INTO `realms_status` VALUES (1784, 1, 'Dark', 1, 1, '2021-07-13 15:14:05', 1, 1);
INSERT INTO `realms_status` VALUES (1785, 1, 'Dark', 1, 1, '2021-07-13 15:14:19', 1, 1);
INSERT INTO `realms_status` VALUES (1786, 1, 'Dark', 1, 1, '2021-07-13 15:14:34', 1, 1);
INSERT INTO `realms_status` VALUES (1787, 1, 'Dark', 1, 1, '2021-07-13 15:14:49', 1, 1);
INSERT INTO `realms_status` VALUES (1788, 1, 'Dark', 1, 1, '2021-07-13 15:15:04', 1, 1);
INSERT INTO `realms_status` VALUES (1789, 1, 'Dark', 1, 1, '2021-07-13 15:15:19', 1, 1);
INSERT INTO `realms_status` VALUES (1790, 1, 'Dark', 1, 1, '2021-07-13 15:15:35', 1, 1);
INSERT INTO `realms_status` VALUES (1791, 1, 'Dark', 1, 1, '2021-07-13 15:15:50', 1, 1);
INSERT INTO `realms_status` VALUES (1792, 1, 'Dark', 1, 1, '2021-07-13 15:16:04', 1, 1);
INSERT INTO `realms_status` VALUES (1793, 1, 'Dark', 1, 1, '2021-07-13 15:16:19', 1, 1);
INSERT INTO `realms_status` VALUES (1794, 1, 'Dark', 1, 1, '2021-07-13 15:16:34', 1, 1);
INSERT INTO `realms_status` VALUES (1795, 1, 'Dark', 1, 1, '2021-07-13 15:16:49', 1, 1);
INSERT INTO `realms_status` VALUES (1796, 1, 'Dark', 1, 1, '2021-07-13 15:17:04', 1, 1);
INSERT INTO `realms_status` VALUES (1797, 1, 'Dark', 1, 1, '2021-07-13 15:17:19', 1, 1);
INSERT INTO `realms_status` VALUES (1798, 1, 'Dark', 1, 1, '2021-07-13 15:17:35', 1, 1);
INSERT INTO `realms_status` VALUES (1799, 1, 'Dark', 1, 1, '2021-07-13 15:17:49', 1, 1);
INSERT INTO `realms_status` VALUES (1800, 1, 'Dark', 1, 1, '2021-07-13 15:18:04', 1, 1);
INSERT INTO `realms_status` VALUES (1801, 1, 'Dark', 1, 1, '2021-07-13 15:18:19', 1, 1);
INSERT INTO `realms_status` VALUES (1802, 1, 'Dark', 1, 1, '2021-07-13 15:18:34', 1, 1);
INSERT INTO `realms_status` VALUES (1803, 1, 'Dark', 1, 1, '2021-07-13 15:18:49', 1, 1);
INSERT INTO `realms_status` VALUES (1804, 1, 'Dark', 1, 1, '2021-07-13 15:19:04', 1, 1);
INSERT INTO `realms_status` VALUES (1805, 1, 'Dark', 1, 1, '2021-07-13 15:19:20', 1, 1);
INSERT INTO `realms_status` VALUES (1806, 1, 'Dark', 1, 1, '2021-07-13 15:19:35', 1, 1);
INSERT INTO `realms_status` VALUES (1807, 1, 'Dark', 1, 1, '2021-07-13 15:19:49', 1, 1);
INSERT INTO `realms_status` VALUES (1808, 1, 'Dark', 1, 1, '2021-07-13 15:20:04', 1, 1);
INSERT INTO `realms_status` VALUES (1809, 1, 'Dark', 1, 1, '2021-07-13 15:20:19', 1, 1);
INSERT INTO `realms_status` VALUES (1810, 1, 'Dark', 1, 1, '2021-07-13 15:20:34', 1, 1);
INSERT INTO `realms_status` VALUES (1811, 1, 'Dark', 1, 1, '2021-07-13 15:20:49', 1, 1);
INSERT INTO `realms_status` VALUES (1812, 1, 'Dark', 1, 1, '2021-07-13 15:21:05', 1, 1);
INSERT INTO `realms_status` VALUES (1813, 1, 'Dark', 1, 1, '2021-07-13 15:21:20', 0, 1);
INSERT INTO `realms_status` VALUES (1814, 1, 'Dark', 1, 1, '2021-07-13 15:21:34', 0, 1);
INSERT INTO `realms_status` VALUES (1815, 1, 'Dark', 1, 1, '2021-07-13 15:21:49', 0, 1);
INSERT INTO `realms_status` VALUES (1816, 1, 'Dark', 1, 1, '2021-07-13 15:22:04', 0, 1);
INSERT INTO `realms_status` VALUES (1817, 1, 'Dark', 1, 1, '2021-07-13 15:22:19', 0, 1);
INSERT INTO `realms_status` VALUES (1818, 1, 'Dark', 1, 1, '2021-07-13 15:22:34', 0, 1);
INSERT INTO `realms_status` VALUES (1819, 1, 'Dark', 1, 1, '2021-07-13 15:22:50', 0, 1);
INSERT INTO `realms_status` VALUES (1820, 1, 'Dark', 1, 1, '2021-07-13 15:23:05', 0, 1);
INSERT INTO `realms_status` VALUES (1821, 1, 'Dark', 1, 1, '2021-07-13 15:23:20', 0, 1);
INSERT INTO `realms_status` VALUES (1822, 1, 'Dark', 1, 1, '2021-07-13 15:23:35', 0, 1);
INSERT INTO `realms_status` VALUES (1823, 1, 'Dark', 1, 1, '2021-07-13 15:23:50', 0, 1);
INSERT INTO `realms_status` VALUES (1824, 1, 'Dark', 1, 1, '2021-07-13 15:24:05', 0, 1);
INSERT INTO `realms_status` VALUES (1825, 1, 'Dark', 1, 1, '2021-07-13 15:24:20', 0, 1);
INSERT INTO `realms_status` VALUES (1826, 1, 'Dark', 1, 1, '2021-07-13 15:24:36', 0, 1);
INSERT INTO `realms_status` VALUES (1827, 1, 'Dark', 1, 1, '2021-07-13 15:24:51', 0, 1);
INSERT INTO `realms_status` VALUES (1828, 1, 'Dark', 1, 1, '2021-07-13 15:25:05', 0, 1);
INSERT INTO `realms_status` VALUES (1829, 1, 'Dark', 1, 1, '2021-07-13 15:25:20', 0, 1);
INSERT INTO `realms_status` VALUES (1830, 1, 'Dark', 1, 1, '2021-07-13 15:25:35', 0, 1);
INSERT INTO `realms_status` VALUES (1831, 1, 'Dark', 1, 1, '2021-07-13 15:25:50', 0, 1);
INSERT INTO `realms_status` VALUES (1832, 1, 'Dark', 1, 1, '2021-07-13 15:26:06', 0, 1);
INSERT INTO `realms_status` VALUES (1833, 1, 'Dark', 1, 1, '2021-07-13 15:26:21', 0, 1);
INSERT INTO `realms_status` VALUES (1834, 1, 'Dark', 1, 1, '2021-07-13 15:26:35', 0, 1);
INSERT INTO `realms_status` VALUES (1835, 1, 'Dark', 1, 1, '2021-07-13 15:26:50', 0, 1);
INSERT INTO `realms_status` VALUES (1836, 1, 'Dark', 1, 1, '2021-07-13 15:27:05', 0, 1);
INSERT INTO `realms_status` VALUES (1837, 1, 'Dark', 1, 1, '2021-07-13 15:27:20', 0, 1);
INSERT INTO `realms_status` VALUES (1838, 1, 'Dark', 1, 1, '2021-07-13 15:27:35', 0, 1);
INSERT INTO `realms_status` VALUES (1839, 1, 'Dark', 1, 1, '2021-07-13 15:27:50', 0, 1);
INSERT INTO `realms_status` VALUES (1840, 1, 'Dark', 1, 1, '2021-07-13 15:28:06', 0, 1);
INSERT INTO `realms_status` VALUES (1841, 1, 'Dark', 1, 1, '2021-07-13 15:28:20', 1, 1);
INSERT INTO `realms_status` VALUES (1842, 1, 'Dark', 1, 1, '2021-07-13 15:28:35', 1, 1);
INSERT INTO `realms_status` VALUES (1843, 1, 'Dark', 1, 1, '2021-07-13 15:28:51', 1, 1);
INSERT INTO `realms_status` VALUES (1844, 1, 'Dark', 1, 1, '2021-07-13 15:29:09', 1, 1);
INSERT INTO `realms_status` VALUES (1845, 1, 'Dark', 1, 1, '2021-07-13 15:29:24', 1, 1);
INSERT INTO `realms_status` VALUES (1846, 1, 'Dark', 1, 1, '2021-07-13 15:31:08', 0, 0);
INSERT INTO `realms_status` VALUES (1847, 1, 'Dark', 1, 1, '2021-07-13 15:32:38', 1, 1);
INSERT INTO `realms_status` VALUES (1848, 1, 'Dark', 1, 1, '2021-07-13 15:32:53', 1, 1);
INSERT INTO `realms_status` VALUES (1849, 1, 'Dark', 1, 1, '2021-07-13 15:33:08', 1, 1);
INSERT INTO `realms_status` VALUES (1850, 1, 'Dark', 1, 1, '2021-07-13 15:33:23', 1, 1);
INSERT INTO `realms_status` VALUES (1851, 1, 'Dark', 1, 1, '2021-07-13 15:33:38', 1, 1);
INSERT INTO `realms_status` VALUES (1852, 1, 'Dark', 1, 1, '2021-07-13 15:33:53', 1, 1);
INSERT INTO `realms_status` VALUES (1853, 1, 'Dark', 1, 1, '2021-07-13 15:34:08', 1, 1);
INSERT INTO `realms_status` VALUES (1854, 1, 'Dark', 1, 1, '2021-07-13 15:34:24', 1, 1);
INSERT INTO `realms_status` VALUES (1855, 1, 'Dark', 1, 1, '2021-07-13 15:34:39', 2, 2);
INSERT INTO `realms_status` VALUES (1856, 1, 'Dark', 1, 1, '2021-07-13 15:34:53', 2, 2);
INSERT INTO `realms_status` VALUES (1857, 1, 'Dark', 1, 1, '2021-07-13 15:35:08', 1, 2);
INSERT INTO `realms_status` VALUES (1858, 1, 'Dark', 1, 1, '2021-07-13 15:35:23', 1, 2);
INSERT INTO `realms_status` VALUES (1859, 1, 'Dark', 1, 1, '2021-07-13 15:35:38', 1, 2);
INSERT INTO `realms_status` VALUES (1860, 1, 'Dark', 1, 1, '2021-07-13 15:35:53', 1, 2);
INSERT INTO `realms_status` VALUES (1861, 1, 'Dark', 1, 1, '2021-07-13 15:36:09', 1, 2);
INSERT INTO `realms_status` VALUES (1862, 1, 'Dark', 1, 1, '2021-07-13 15:36:24', 1, 2);
INSERT INTO `realms_status` VALUES (1863, 1, 'Dark', 1, 1, '2021-07-13 15:36:38', 2, 2);
INSERT INTO `realms_status` VALUES (1864, 1, 'Dark', 1, 1, '2021-07-13 15:36:53', 2, 2);
INSERT INTO `realms_status` VALUES (1865, 1, 'Dark', 1, 1, '2021-07-13 15:37:08', 0, 2);
INSERT INTO `realms_status` VALUES (1866, 1, 'Dark', 1, 1, '2021-07-13 15:37:23', 0, 2);
INSERT INTO `realms_status` VALUES (1867, 1, 'Dark', 1, 1, '2021-07-13 15:37:38', 0, 2);
INSERT INTO `realms_status` VALUES (1868, 1, 'Dark', 1, 1, '2021-07-13 15:37:54', 0, 2);
INSERT INTO `realms_status` VALUES (1869, 1, 'Dark', 1, 1, '2021-07-13 15:38:08', 0, 2);
INSERT INTO `realms_status` VALUES (1870, 1, 'Dark', 1, 1, '2021-07-13 15:38:23', 0, 2);
INSERT INTO `realms_status` VALUES (1871, 1, 'Dark', 1, 1, '2021-07-13 15:38:38', 0, 2);
INSERT INTO `realms_status` VALUES (1872, 1, 'Dark', 1, 1, '2021-07-13 15:38:53', 0, 2);
INSERT INTO `realms_status` VALUES (1873, 1, 'Dark', 1, 1, '2021-07-13 15:39:08', 0, 2);
INSERT INTO `realms_status` VALUES (1874, 1, 'Dark', 1, 1, '2021-07-13 15:39:23', 0, 2);
INSERT INTO `realms_status` VALUES (1875, 1, 'Dark', 1, 1, '2021-07-13 15:39:39', 0, 2);
INSERT INTO `realms_status` VALUES (1876, 1, 'Dark', 1, 1, '2021-07-13 15:39:54', 0, 2);
INSERT INTO `realms_status` VALUES (1877, 1, 'Dark', 1, 1, '2021-07-13 15:40:08', 0, 2);
INSERT INTO `realms_status` VALUES (1878, 1, 'Dark', 1, 1, '2021-07-13 15:40:23', 0, 2);
INSERT INTO `realms_status` VALUES (1879, 1, 'Dark', 1, 1, '2021-07-13 15:40:38', 0, 2);
INSERT INTO `realms_status` VALUES (1880, 1, 'Dark', 1, 1, '2021-07-13 15:40:53', 0, 2);
INSERT INTO `realms_status` VALUES (1881, 1, 'Dark', 1, 1, '2021-07-13 15:41:08', 0, 2);
INSERT INTO `realms_status` VALUES (1882, 1, 'Dark', 1, 1, '2021-07-13 15:41:24', 0, 2);
INSERT INTO `realms_status` VALUES (1883, 1, 'Dark', 1, 1, '2021-07-13 15:41:39', 0, 2);
INSERT INTO `realms_status` VALUES (1884, 1, 'Dark', 1, 1, '2021-07-13 15:41:53', 1, 2);
INSERT INTO `realms_status` VALUES (1885, 1, 'Dark', 1, 1, '2021-07-13 15:42:08', 1, 2);
INSERT INTO `realms_status` VALUES (1886, 1, 'Dark', 1, 1, '2021-07-13 15:42:23', 1, 2);
INSERT INTO `realms_status` VALUES (1887, 1, 'Dark', 1, 1, '2021-07-13 15:42:38', 0, 2);
INSERT INTO `realms_status` VALUES (1888, 1, 'Dark', 1, 1, '2021-07-13 15:42:53', 0, 2);
INSERT INTO `realms_status` VALUES (1889, 1, 'Dark', 1, 1, '2021-07-13 15:43:09', 0, 2);
INSERT INTO `realms_status` VALUES (1890, 1, 'Dark', 1, 1, '2021-07-13 15:43:24', 0, 2);
INSERT INTO `realms_status` VALUES (1891, 1, 'Dark', 1, 1, '2021-07-13 15:43:38', 0, 2);
INSERT INTO `realms_status` VALUES (1892, 1, 'Dark', 1, 1, '2021-07-13 15:43:53', 0, 2);
INSERT INTO `realms_status` VALUES (1893, 1, 'Dark', 1, 1, '2021-07-13 15:44:08', 0, 2);
INSERT INTO `realms_status` VALUES (1894, 1, 'Dark', 1, 1, '2021-07-13 15:44:23', 0, 2);
INSERT INTO `realms_status` VALUES (1895, 1, 'Dark', 1, 1, '2021-07-13 15:44:38', 0, 2);
INSERT INTO `realms_status` VALUES (1896, 1, 'Dark', 1, 1, '2021-07-13 15:44:54', 0, 2);
INSERT INTO `realms_status` VALUES (1897, 1, 'Dark', 1, 1, '2021-07-13 15:45:09', 0, 2);
INSERT INTO `realms_status` VALUES (1898, 1, 'Dark', 1, 1, '2021-07-13 15:45:23', 0, 2);
INSERT INTO `realms_status` VALUES (1899, 1, 'Dark', 1, 1, '2021-07-13 15:45:38', 0, 2);
INSERT INTO `realms_status` VALUES (1900, 1, 'Dark', 1, 1, '2021-07-13 15:45:53', 0, 2);
INSERT INTO `realms_status` VALUES (1901, 1, 'Dark', 1, 1, '2021-07-13 15:46:08', 0, 2);
INSERT INTO `realms_status` VALUES (1902, 1, 'Dark', 1, 1, '2021-07-13 15:46:23', 0, 2);
INSERT INTO `realms_status` VALUES (1903, 1, 'Dark', 1, 1, '2021-07-13 15:46:39', 0, 2);
INSERT INTO `realms_status` VALUES (1904, 1, 'Dark', 1, 1, '2021-07-13 15:46:54', 0, 2);
INSERT INTO `realms_status` VALUES (1905, 1, 'Dark', 1, 1, '2021-07-13 15:47:08', 0, 2);
INSERT INTO `realms_status` VALUES (1906, 1, 'Dark', 1, 1, '2021-07-13 15:47:23', 0, 2);
INSERT INTO `realms_status` VALUES (1907, 1, 'Dark', 1, 1, '2021-07-13 15:47:38', 0, 2);
INSERT INTO `realms_status` VALUES (1908, 1, 'Dark', 1, 1, '2021-07-13 15:47:53', 0, 2);
INSERT INTO `realms_status` VALUES (1909, 1, 'Dark', 1, 1, '2021-07-13 15:48:08', 0, 2);
INSERT INTO `realms_status` VALUES (1910, 1, 'Dark', 1, 1, '2021-07-13 15:48:23', 0, 2);
INSERT INTO `realms_status` VALUES (1911, 1, 'Dark', 1, 1, '2021-07-13 15:48:39', 0, 2);
INSERT INTO `realms_status` VALUES (1912, 1, 'Dark', 1, 1, '2021-07-13 15:48:54', 0, 2);
INSERT INTO `realms_status` VALUES (1913, 1, 'Dark', 1, 1, '2021-07-13 15:49:51', 0, 0);
INSERT INTO `realms_status` VALUES (1914, 1, 'Dark', 1, 1, '2021-07-13 15:50:06', 0, 0);
INSERT INTO `realms_status` VALUES (1915, 1, 'Dark', 1, 1, '2021-07-13 15:50:20', 0, 0);
INSERT INTO `realms_status` VALUES (1916, 1, 'Dark', 1, 1, '2021-07-13 15:50:35', 0, 1);
INSERT INTO `realms_status` VALUES (1917, 1, 'Dark', 1, 1, '2021-07-13 15:50:50', 0, 1);
INSERT INTO `realms_status` VALUES (1918, 1, 'Dark', 1, 1, '2021-07-13 15:51:05', 0, 1);
INSERT INTO `realms_status` VALUES (1919, 1, 'Dark', 1, 1, '2021-07-13 15:51:43', 0, 0);
INSERT INTO `realms_status` VALUES (1920, 1, 'Dark', 1, 1, '2021-07-13 15:51:59', 1, 1);
INSERT INTO `realms_status` VALUES (1921, 1, 'Dark', 1, 1, '2021-07-13 15:52:14', 1, 1);
INSERT INTO `realms_status` VALUES (1922, 1, 'Dark', 1, 1, '2021-07-13 15:52:28', 1, 1);
INSERT INTO `realms_status` VALUES (1923, 1, 'Dark', 1, 1, '2021-07-13 15:52:43', 1, 1);
INSERT INTO `realms_status` VALUES (1924, 1, 'Dark', 1, 1, '2021-07-13 15:52:58', 1, 1);
INSERT INTO `realms_status` VALUES (1925, 1, 'Dark', 1, 1, '2021-07-13 15:53:13', 1, 1);
INSERT INTO `realms_status` VALUES (1926, 1, 'Dark', 1, 1, '2021-07-13 15:53:28', 1, 1);
INSERT INTO `realms_status` VALUES (1927, 1, 'Dark', 1, 1, '2021-07-13 15:53:43', 1, 1);
INSERT INTO `realms_status` VALUES (1928, 1, 'Dark', 1, 1, '2021-07-13 15:53:59', 1, 1);
INSERT INTO `realms_status` VALUES (1929, 1, 'Dark', 1, 1, '2021-07-13 15:54:14', 1, 1);
INSERT INTO `realms_status` VALUES (1930, 1, 'Dark', 1, 1, '2021-07-13 15:54:29', 1, 1);
INSERT INTO `realms_status` VALUES (1931, 1, 'Dark', 1, 1, '2021-07-13 15:54:43', 1, 1);
INSERT INTO `realms_status` VALUES (1932, 1, 'Dark', 1, 1, '2021-07-13 15:54:58', 1, 1);
INSERT INTO `realms_status` VALUES (1933, 1, 'Dark', 1, 1, '2021-07-13 15:55:13', 1, 1);
INSERT INTO `realms_status` VALUES (1934, 1, 'Dark', 1, 1, '2021-07-13 15:55:28', 1, 1);
INSERT INTO `realms_status` VALUES (1935, 1, 'Dark', 1, 1, '2021-07-13 15:55:43', 1, 1);
INSERT INTO `realms_status` VALUES (1936, 1, 'Dark', 1, 1, '2021-07-13 15:55:58', 1, 1);

-- ----------------------------
-- Table structure for records_family
-- ----------------------------
DROP TABLE IF EXISTS `records_family`;
CREATE TABLE `records_family`  (
  `Identity` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `FamilyIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Name` varchar(64) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT '',
  `LeaderIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Count` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Money` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL,
  `DeletedAt` datetime NULL DEFAULT NULL,
  `ChallengeMap` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `DominatedMap` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Level` tinyint(3) UNSIGNED NOT NULL DEFAULT 0,
  `BpTower` tinyint(3) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of records_family
-- ----------------------------

-- ----------------------------
-- Table structure for records_guild_war
-- ----------------------------
DROP TABLE IF EXISTS `records_guild_war`;
CREATE TABLE `records_guild_war`  (
  `Identity` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `SyndicateIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `LeaderIdentity` int(10) UNSIGNED NOT NULL,
  `Date` datetime NOT NULL,
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of records_guild_war
-- ----------------------------

-- ----------------------------
-- Table structure for records_syndicate
-- ----------------------------
DROP TABLE IF EXISTS `records_syndicate`;
CREATE TABLE `records_syndicate`  (
  `Id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `SyndicateIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LeaderIdentity` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `Count` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL,
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of records_syndicate
-- ----------------------------

-- ----------------------------
-- Table structure for records_user
-- ----------------------------
DROP TABLE IF EXISTS `records_user`;
CREATE TABLE `records_user`  (
  `Id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `UserIdentity` int(11) UNSIGNED NOT NULL,
  `AccountIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MateId` int(4) UNSIGNED NOT NULL,
  `Level` tinyint(1) UNSIGNED NOT NULL DEFAULT 1,
  `Experience` bigint(16) UNSIGNED NOT NULL DEFAULT 0,
  `Profession` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `OldProfession` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `NewProfession` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `Metempsychosis` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `Strength` smallint(2) UNSIGNED NOT NULL DEFAULT 0,
  `Agility` smallint(2) UNSIGNED NOT NULL DEFAULT 0,
  `Vitality` smallint(2) UNSIGNED NOT NULL DEFAULT 0,
  `Spirit` smallint(2) UNSIGNED NOT NULL DEFAULT 0,
  `AdditionalPoints` smallint(2) UNSIGNED NOT NULL DEFAULT 0,
  `SyndicateIdentity` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `SyndicatePosition` smallint(2) UNSIGNED NOT NULL DEFAULT 0,
  `NobilityDonation` bigint(16) UNSIGNED NOT NULL DEFAULT 0,
  `NobilityRank` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `SupermanCount` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `DeletedAt` datetime NULL DEFAULT NULL,
  `Money` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `WarehouseMoney` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `ConquerPoints` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `FamilyIdentity` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `FamilyRank` smallint(5) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `IdIdx`(`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 61 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of records_user
-- ----------------------------
INSERT INTO `records_user` VALUES (3, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (4, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (5, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (6, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (7, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (8, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (9, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (10, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (11, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (12, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (13, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (14, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (15, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (16, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (17, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (18, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (19, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (20, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (21, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (22, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (23, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (24, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (25, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (26, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (27, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (28, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (29, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (30, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (31, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (32, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (33, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (34, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (35, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (36, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (37, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (38, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (39, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (40, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (41, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (42, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (43, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (44, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (45, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (46, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (47, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (48, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (49, 1, 0, 0, 'Inburst2', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (50, 1, 0, 0, 'Inburst2', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (51, 1, 0, 0, 'Inburst2', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (52, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (53, 1, 0, 0, 'Inburst2', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (54, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (55, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (56, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (57, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (58, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (59, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);
INSERT INTO `records_user` VALUES (60, 1, 0, 0, 'Inburst', 0, 1, 0, 40, 0, 0, 0, 4, 6, 12, 0, 0, 0, 0, 0, 0, 0, NULL, 1000, 0, 0, 0, 0);

-- ----------------------------
-- Table structure for reward_type
-- ----------------------------
DROP TABLE IF EXISTS `reward_type`;
CREATE TABLE `reward_type`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Type` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'StrUnknown',
  `ActionId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `ItemType` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Money` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `ConquerPoints` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of reward_type
-- ----------------------------

-- ----------------------------
-- Table structure for reward_user
-- ----------------------------
DROP TABLE IF EXISTS `reward_user`;
CREATE TABLE `reward_user`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `AccountId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `RewardId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `ClaimerId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `ClaimedAt` datetime NULL DEFAULT NULL,
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of reward_user
-- ----------------------------

-- ----------------------------
-- Table structure for shop_checkout
-- ----------------------------
DROP TABLE IF EXISTS `shop_checkout`;
CREATE TABLE `shop_checkout`  (
  `Id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `UserId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Comment` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `BillingInformationId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `PaymentType` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `TransactionToken` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PaidAt` datetime NULL DEFAULT NULL,
  `PaymentMethodType` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `PaymentMethodCode` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `PaymentUrl` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TransactionStatus` int(11) NOT NULL DEFAULT 0,
  `ClientName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientEmail` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientAddress` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientNumber` int(11) NOT NULL DEFAULT 0,
  `ClientComplement` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientDistrict` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientCity` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientState` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientPostalCode` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClientPhone` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `CancelationSource` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1000000034 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shop_checkout
-- ----------------------------

-- ----------------------------
-- Table structure for shop_checkout_items
-- ----------------------------
DROP TABLE IF EXISTS `shop_checkout_items`;
CREATE TABLE `shop_checkout_items`  (
  `Id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `CheckoutId` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `ProductId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Amount` int(10) UNSIGNED NOT NULL DEFAULT 1,
  `Value` double NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `CheckoutId`(`CheckoutId`) USING BTREE,
  CONSTRAINT `ChkItems` FOREIGN KEY (`CheckoutId`) REFERENCES `shop_checkout` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE = InnoDB AUTO_INCREMENT = 44 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shop_checkout_items
-- ----------------------------

-- ----------------------------
-- Table structure for shop_checkout_status_tracking
-- ----------------------------
DROP TABLE IF EXISTS `shop_checkout_status_tracking`;
CREATE TABLE `shop_checkout_status_tracking`  (
  `Id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `CheckoutId` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `TransactionId` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `NotificationCode` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `NewStatus` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Date` datetime NOT NULL,
  `UpdatedAt` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `CheckoutID`(`CheckoutId`) USING BTREE,
  CONSTRAINT `CheckoutID` FOREIGN KEY (`CheckoutId`) REFERENCES `shop_checkout` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB AUTO_INCREMENT = 11 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shop_checkout_status_tracking
-- ----------------------------

-- ----------------------------
-- Table structure for shop_products
-- ----------------------------
DROP TABLE IF EXISTS `shop_products`;
CREATE TABLE `shop_products`  (
  `id` int(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  `type` tinyint(1) UNSIGNED NOT NULL DEFAULT 0,
  `name` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `description` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `long_description` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `value` double(20, 2) UNSIGNED NOT NULL DEFAULT 0.00,
  `data0` int(11) NOT NULL DEFAULT 0,
  `data1` int(11) NOT NULL DEFAULT 0,
  `data2` int(11) NOT NULL DEFAULT 0,
  `data3` int(11) NOT NULL DEFAULT 0,
  `data4` int(11) NOT NULL DEFAULT 0,
  `data5` int(11) NOT NULL DEFAULT 0,
  `data6` int(11) NOT NULL DEFAULT 0,
  `data7` int(11) NOT NULL DEFAULT 0,
  `img_thumb` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `img_main` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `flag` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `from` datetime NULL DEFAULT NULL,
  `to` datetime NULL DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1000000052 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shop_products
-- ----------------------------

-- ----------------------------
-- Table structure for shop_products_img
-- ----------------------------
DROP TABLE IF EXISTS `shop_products_img`;
CREATE TABLE `shop_products_img`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `type` tinyint(1) NOT NULL,
  `folder` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `url` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shop_products_img
-- ----------------------------

-- ----------------------------
-- Table structure for support_tickets
-- ----------------------------
DROP TABLE IF EXISTS `support_tickets`;
CREATE TABLE `support_tickets`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `UserId` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `Subject` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT '',
  `Content` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Urgency` tinyint(3) UNSIGNED NOT NULL DEFAULT 0,
  `RequireAdm` tinyint(3) UNSIGNED NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `LastReply` datetime NULL DEFAULT NULL,
  `DeletedAt` datetime NULL DEFAULT NULL,
  `Flag` int(11) NOT NULL DEFAULT 0,
  `Status` int(4) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of support_tickets
-- ----------------------------

-- ----------------------------
-- Table structure for support_tickets_answers
-- ----------------------------
DROP TABLE IF EXISTS `support_tickets_answers`;
CREATE TABLE `support_tickets_answers`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `TicketId` int(10) UNSIGNED NULL DEFAULT 0,
  `AuthorId` int(10) UNSIGNED NULL DEFAULT 0,
  `Message` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Solution` tinyint(3) UNSIGNED NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `DeletedAt` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of support_tickets_answers
-- ----------------------------

-- ----------------------------
-- Table structure for web_exception_handing
-- ----------------------------
DROP TABLE IF EXISTS `web_exception_handing`;
CREATE TABLE `web_exception_handing`  (
  `Id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Path` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Message` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `StackTrace` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Date` datetime NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of web_exception_handing
-- ----------------------------

-- ----------------------------
-- Procedure structure for GetTopGuildWinners
-- ----------------------------
DROP PROCEDURE IF EXISTS `GetTopGuildWinners`;
delimiter ;;
CREATE PROCEDURE `GetTopGuildWinners`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	SELECT
		guild.`Name` `SyndicateName`,
		IFNULL( usr.`Name`, "StrUnknown" ) `LeaderName`,
		guild.Count `MemberCount`,
		( SELECT COUNT( gw.SyndicateIdentity ) FROM records_guild_war gw WHERE gw.SyndicateIdentity = guild.SyndicateIdentity ) `GuildWars` 
	FROM
		records_syndicate guild
		LEFT JOIN records_user usr ON usr.UserIdentity = guild.LeaderIdentity 
		WHERE (idServer = -1 OR guild.ServerIdentity = idServer)
		LIMIT maxLimit;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for GetTopMoneybag
-- ----------------------------
DROP PROCEDURE IF EXISTS `GetTopMoneybag`;
delimiter ;;
CREATE PROCEDURE `GetTopMoneybag`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		(ru0.Money + ru0.WarehouseMoney) `Moneybag`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.Id
		LEFT JOIN records_syndicate syn ON syn.Id=ru0.SyndicateIdentity
	WHERE 
		ru0.`Name` NOT LIKE "%[%]%"
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY (ru0.Money + ru0.WarehouseMoney) DESC
	LIMIT maxLimitFrom, maxLimit;	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for GetTopNoble
-- ----------------------------
DROP PROCEDURE IF EXISTS `GetTopNoble`;
delimiter ;;
CREATE PROCEDURE `GetTopNoble`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		ru0.NobilityDonation `NobleDonation`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.Id
		LEFT JOIN records_syndicate syn ON syn.Id=ru0.SyndicateIdentity
	WHERE 
		ru0.NobilityDonation > 3000000
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.NobilityDonation DESC
	LIMIT maxLimitFrom, maxLimit;	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for GetTopPlayers
-- ----------------------------
DROP PROCEDURE IF EXISTS `GetTopPlayers`;
delimiter ;;
CREATE PROCEDURE `GetTopPlayers`(IN minLevel INT, IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.UserIdentity
		LEFT JOIN records_syndicate syn ON syn.SyndicateIdentity=ru0.SyndicateIdentity
	WHERE 
		ru0.`Level` >= minLevel
		AND ru0.`Name` NOT LIKE "%[%]%"
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.`Level` DESC, ru0.Experience DESC
	LIMIT maxLimitFrom, maxLimit;	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for GetTopProfession
-- ----------------------------
DROP PROCEDURE IF EXISTS `GetTopProfession`;
delimiter ;;
CREATE PROCEDURE `GetTopProfession`(IN minLevel INT, IN fromProf INT, IN toProf INT, IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.UserIdentity
		LEFT JOIN records_syndicate syn ON syn.SyndicateIdentity=ru0.SyndicateIdentity
	WHERE 
		ru0.Profession BETWEEN fromProf AND toProf
		AND ru0.`Level` >= minLevel		
		AND ru0.`Name` NOT LIKE "%[%]%"
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.`Level` DESC, ru0.Experience DESC
	LIMIT maxLimitFrom, maxLimit;	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for GetTopSuperman
-- ----------------------------
DROP PROCEDURE IF EXISTS `GetTopSuperman`;
delimiter ;;
CREATE PROCEDURE `GetTopSuperman`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		ru0.SupermanCount `SupermanCount`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.Id
		LEFT JOIN records_syndicate syn ON syn.Id=ru0.SyndicateIdentity
	WHERE 
		ru0.`Name` NOT LIKE "%[%]%" AND ru0.SupermanCount > 0		
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.`SupermanCount` DESC
	LIMIT maxLimitFrom, maxLimit;	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for GetTopSyndicate
-- ----------------------------
DROP PROCEDURE IF EXISTS `GetTopSyndicate`;
delimiter ;;
CREATE PROCEDURE `GetTopSyndicate`(IN minMembers INT, IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		syn.`Name` `Name`,
		usr.`Name` `LeaderName`,
		syn.Count `SyndicateCount`,
		(SELECT COUNT(*) FROM records_guild_war rgw WHERE rgw.SyndicateIdentity=syn.SyndicateIdentity) `GuildWarCount`
	FROM
		records_syndicate syn
		LEFT JOIN records_user usr ON usr.UserIdentity=syn.LeaderIdentity AND usr.`Name` NOT LIKE "%[%]%"		
	WHERE 
		syn.Count >= minMembers
		AND ISNULL(syn.DeletedAt)
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY syn.`Count` DESC
	LIMIT maxLimitFrom, maxLimit;	
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table account
-- ----------------------------
DROP TRIGGER IF EXISTS `account_passwordhash`;
delimiter ;;
CREATE TRIGGER `account_passwordhash` BEFORE INSERT ON `account` FOR EACH ROW BEGIN
    --
	-- Name:   Password Hash
	-- Author: Gareth Jensen (Spirited)
	-- Date:   2018-09-25
	--
	-- Description:
	-- When a plain text password without a hash or salt has been inserted into the database 
	-- along with a new account, then the plain text password will be hashed and a salt will
	-- be generated from a random MD5 string. Due to client limitations, passwords cannot be
    -- longer than 16 characters.
	-- 
	IF (NEW.`Salt` IS NULL) THEN
    
		IF (LENGTH(NEW.`Password`) > 16) THEN
			SET NEW.`Password` = NULL;
        END IF;
        
        SET NEW.`Salt` = MD5(RAND());
        SET NEW.`Password` = SHA2(CONCAT(NEW.`Password`, NEW.`Salt`), 256);
    END IF;
END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
