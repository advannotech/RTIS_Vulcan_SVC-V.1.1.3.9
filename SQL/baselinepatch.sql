CREATE PROC Check_First_Coat
@vCatalystCode VARCHAR(225),
@vLotNumber VARCHAR(225)
AS
SELECT *
FROM [tbl_RTIS_Zect_Raws] zr, [tbl_RTIS_Zect_Jobs] zj
WHERE zr.vCatalystCode=@vCatalystCode AND zr.vCatalystCode  LIKE '%1st%' AND @vCatalystCode  NOT LIKE '%2nd%' AND @vCatalystCode  NOT LIKE '%3rd%' AND @vCatalystCode NOT LIKE '%4th%'
AND(zr.[vRMCode] = zj.vSlurryCode AND vLotNumber = @vLotNumber)
ORDER BY vLotNumber;

CREATE PROC Check_Second_Coat
@vCatalystCode VARCHAR(225),
@vLotNumber VARCHAR(225)
AS
SELECT *
FROM [tbl_RTIS_Zect_Raws] zr, [tbl_RTIS_Zect_Jobs] zj
WHERE zr.vCatalystCode=@vCatalystCode AND zr.vCatalystCode LIKE '%2nd%' AND @vCatalystCode  NOT LIKE '%1st%' AND @vCatalystCode  NOT LIKE '%3rd%' AND @vCatalystCode NOT LIKE '%4th%'
AND(zr.[vRMCode] = zj.vSlurryCode AND vLotNumber = @vLotNumber)
ORDER BY vLotNumber;


CREATE PROC Check_Third_Coat
@vCatalystCode VARCHAR(225),
@vLotNumber VARCHAR(225)
AS
SELECT *
FROM [tbl_RTIS_Zect_Raws] zr, [tbl_RTIS_Zect_Jobs] zj
WHERE zr.vCatalystCode=@vCatalystCode AND zr.vCatalystCode LIKE '%3rd%' AND @vCatalystCode  NOT LIKE '%1st%' AND @vCatalystCode  NOT LIKE '%2nd%' AND @vCatalystCode NOT LIKE '%4th%'
AND(zr.[vRMCode] = zj.vSlurryCode AND vLotNumber = @vLotNumber)
ORDER BY vLotNumber;


CREATE PROC Check_Fourth_Coat
@vCatalystCode VARCHAR(225),
@vLotNumber VARCHAR(225)
AS
SELECT *
FROM [tbl_RTIS_Zect_Raws] zr, [tbl_RTIS_Zect_Jobs] zj
WHERE zr.vCatalystCode=@vCatalystCode AND zr.vCatalystCode LIKE '%4th%' AND @vCatalystCode  NOT LIKE '%1st%' AND @vCatalystCode  NOT LIKE '%2nd%' AND @vCatalystCode NOT LIKE '%3rd%'
AND(zr.[vRMCode] = zj.vSlurryCode AND vLotNumber = @vLotNumber)
ORDER BY vLotNumber;
