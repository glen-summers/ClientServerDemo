-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema demo
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `demo` ;

-- -----------------------------------------------------
-- Schema demo
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `demo` DEFAULT CHARACTER SET utf8 ;
USE `demo` ;

-- -----------------------------------------------------
-- Table `data`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `data` ;

CREATE TABLE IF NOT EXISTS `data` (
  `Id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `DataValue` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8;

USE `demo` ;

-- -----------------------------------------------------
-- procedure AddData
-- -----------------------------------------------------

USE `demo`;
DROP procedure IF EXISTS `AddData`;

DELIMITER $$
USE `demo`$$
CREATE DEFINER=`DemoUser`@`localhost` PROCEDURE `AddData`(IN `dataValue` VARCHAR(255), OUT `id` INT)
BEGIN
INSERT INTO Data (`DataValue`) VALUES (`dataValue`);
SET `id` = last_insert_id();
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure Query
-- -----------------------------------------------------

USE `demo`;
DROP procedure IF EXISTS `Query`;

DELIMITER $$
USE `demo`$$
CREATE DEFINER=`DemoUser`@`localhost` PROCEDURE `Query`(IN `count` INT UNSIGNED)
BEGIN
SELECT `Id`,`DataValue` from Data ORDER BY `Id` DESC limit `count`;
END$$

DELIMITER ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
