USE [master]
GO
CREATE DATABASE [OtomasyonDB]
GO
ALTER DATABASE [OtomasyonDB] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [OtomasyonDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [OtomasyonDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [OtomasyonDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [OtomasyonDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [OtomasyonDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [OtomasyonDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [OtomasyonDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [OtomasyonDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [OtomasyonDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [OtomasyonDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [OtomasyonDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [OtomasyonDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [OtomasyonDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [OtomasyonDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [OtomasyonDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [OtomasyonDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [OtomasyonDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [OtomasyonDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [OtomasyonDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [OtomasyonDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [OtomasyonDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [OtomasyonDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [OtomasyonDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [OtomasyonDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [OtomasyonDB] SET  MULTI_USER 
GO
ALTER DATABASE [OtomasyonDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [OtomasyonDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [OtomasyonDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [OtomasyonDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [OtomasyonDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [OtomasyonDB] SET QUERY_STORE = OFF
GO
USE [OtomasyonDB]
GO
/****** Object:  UserDefinedFunction [dbo].[PhoneFormat]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[PhoneFormat] 

(@Phone nvarchar(20)) 

returns nvarchar(20)

as

begin



Declare @ReturnPhone nvarchar(20);



Set @ReturnPhone = (



Case 



When len(replace(@Phone,' ',''))=10 

then LEFT(replace(@Phone, ' ',''), 3) + '-' + SUBSTRING(replace(@Phone, ' ',''), 4,3) + '-' + 

SUBSTRING(replace(@Phone, ' ',''), 7,2) + '-' + RIGHT(replace(@Phone, ' ',''),2) 



When  len(replace(@Phone,' ',''))=11 

then SUBSTRING(replace(@Phone, ' ',''), 2,3) + '-' + SUBSTRING(replace(@Phone, ' ',''), 5,3) + '-' + 

SUBSTRING(replace(@Phone, ' ',''), 8,2) + '-' + RIGHT(replace(@Phone, ' ',''),2) 



When  len(replace(@Phone,' ',''))=7 

then  LEFT(replace('000' + @Phone, ' ',''), 3) + '-' + SUBSTRING(replace('000' + @Phone, ' ',''), 4,3) + '-' +

SUBSTRING(replace('000' + @Phone, ' ',''), 7,2) + '-' + RIGHT(replace('000' + @Phone, ' ',''),2) 



else @Phone 

end

);

return @ReturnPhone;

end


GO
/****** Object:  Table [dbo].[Birimler]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Birimler](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Adi] [nvarchar](30) NULL,
	[Kisaltma] [nchar](3) NULL,
 CONSTRAINT [PK_Birimler] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gelir_gider]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gelir_gider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Adi] [nvarchar](100) NULL,
	[Tarih] [datetime] NULL,
	[Tutar] [decimal](9, 2) NULL,
 CONSTRAINT [PK__Gelir_gi__3214EC0764B04C80] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Isletme]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Isletme](
	[Adi] [nvarchar](60) NULL,
	[Telefon] [nvarchar](25) NULL,
	[Email] [nvarchar](60) NULL,
	[Adres] [nvarchar](400) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Kullanicilar]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kullanicilar](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Adi] [nvarchar](50) NOT NULL,
	[Sifre] [nvarchar](50) NOT NULL,
	[Yetki] [smallint] NOT NULL,
 CONSTRAINT [PK_Kullanicilar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Musteriler]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Musteriler](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Adi] [nvarchar](50) NULL,
	[Telefon] [nvarchar](50) NULL,
	[Son_Alisveris_Tarihi] [datetime] NULL,
	[Borc] [decimal](9, 2) NULL,
	[Toplam_Alisveris] [decimal](9, 2) NULL,
 CONSTRAINT [PK_Musteriler] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Satis_Detayi]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Satis_Detayi](
	[Id] [int] NOT NULL,
	[Barkod_Kodu] [nvarchar](25) NULL,
	[Miktar] [decimal](9, 1) NULL,
	[BirimId] [tinyint] NULL,
	[Iade] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Satislar]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Satislar](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Musteri_Id] [int] NOT NULL,
	[Tarih] [datetime] NOT NULL,
	[Satis_Bedeli] [decimal](9, 2) NOT NULL,
	[Satis_Kari] [decimal](9, 2) NOT NULL,
	[Kasiyer_Id] [int] NOT NULL,
	[KasadakiPara] [decimal](18, 2) NULL,
	[Nakit] [decimal](9, 2) NULL,
	[Kredi] [decimal](9, 2) NULL,
	[Taksit] [tinyint] NULL,
 CONSTRAINT [PK_Satislar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Urunler]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Urunler](
	[Bakod_kodu] [nvarchar](25) NOT NULL,
	[Adi] [nvarchar](100) NULL,
	[Stok] [decimal](9, 1) NULL,
	[Maliyet] [decimal](9, 2) NULL,
	[Satis_fiyati] [decimal](9, 2) NULL,
	[Stok_birimi] [tinyint] NULL,
	[Kritik_miktar] [decimal](9, 1) NULL,
	[Hizli_urun] [bit] NOT NULL,
 CONSTRAINT [PK_Urunler] PRIMARY KEY CLUSTERED 
(
	[Bakod_kodu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Birimler] ON 

INSERT [dbo].[Birimler] ([Id], [Adi], [Kisaltma]) VALUES (1, N'Adet', N'Adt')
INSERT [dbo].[Birimler] ([Id], [Adi], [Kisaltma]) VALUES (2, N'KİLO', N'KG ')
SET IDENTITY_INSERT [dbo].[Birimler] OFF
GO
SET IDENTITY_INSERT [dbo].[Kullanicilar] ON 

INSERT [dbo].[Kullanicilar] ([Id], [Adi], [Sifre], [Yetki]) VALUES (3, N'Muhammed', N'123456', 1)
SET IDENTITY_INSERT [dbo].[Kullanicilar] OFF
GO
SET IDENTITY_INSERT [dbo].[Musteriler] ON 

INSERT [dbo].[Musteriler] ([Id], [Adi], [Telefon], [Son_Alisveris_Tarihi], [Borc], [Toplam_Alisveris]) VALUES (1, N'Kaydedilmedi', N'000-000-00-00', CAST(N'2020-01-01T00:00:00.000' AS DateTime), CAST(0.00 AS Decimal(9, 2)), CAST(0.00 AS Decimal(9, 2)))
SET IDENTITY_INSERT [dbo].[Musteriler] OFF
GO
INSERT [dbo].[Urunler] ([Bakod_kodu], [Adi], [Stok], [Maliyet], [Satis_fiyati], [Stok_birimi], [Kritik_miktar], [Hizli_urun]) VALUES (N'0', N'Bu Ürün Silindi', CAST(0.0 AS Decimal(9, 1)), CAST(0.00 AS Decimal(9, 2)), CAST(0.00 AS Decimal(9, 2)), 1, CAST(0.0 AS Decimal(9, 1)), 0)
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Kullanic__C69666A94EB0ABDA]    Script Date: 31.10.2020 05:47:49 ******/
ALTER TABLE [dbo].[Kullanicilar] ADD UNIQUE NONCLUSTERED 
(
	[Adi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Satislar] ADD  CONSTRAINT [DF_Satislar_Satis_Bedeli]  DEFAULT ((0)) FOR [Satis_Bedeli]
GO
ALTER TABLE [dbo].[Satislar] ADD  CONSTRAINT [DF_Satislar_Satis_Kari]  DEFAULT ((0)) FOR [Satis_Kari]
GO
ALTER TABLE [dbo].[Satislar] ADD  CONSTRAINT [DF_Satislar_Nakit]  DEFAULT ((0)) FOR [Nakit]
GO
ALTER TABLE [dbo].[Satislar] ADD  CONSTRAINT [DF_Satislar_Kredi]  DEFAULT ((0)) FOR [Kredi]
GO
ALTER TABLE [dbo].[Satislar] ADD  CONSTRAINT [DF_Satislar_Taksit]  DEFAULT ((0)) FOR [Taksit]
GO
ALTER TABLE [dbo].[Urunler] ADD  CONSTRAINT [DF_Urunler_Hizli_urun]  DEFAULT ((0)) FOR [Hizli_urun]
GO
ALTER TABLE [dbo].[Satis_Detayi]  WITH CHECK ADD  CONSTRAINT [FK_Satis_Detayi_Satislar] FOREIGN KEY([Id])
REFERENCES [dbo].[Satislar] ([Id])
GO
ALTER TABLE [dbo].[Satis_Detayi] CHECK CONSTRAINT [FK_Satis_Detayi_Satislar]
GO
ALTER TABLE [dbo].[Satis_Detayi]  WITH CHECK ADD  CONSTRAINT [FK_Satis_Detayi_Urunler] FOREIGN KEY([Barkod_Kodu])
REFERENCES [dbo].[Urunler] ([Bakod_kodu])
GO
ALTER TABLE [dbo].[Satis_Detayi] CHECK CONSTRAINT [FK_Satis_Detayi_Urunler]
GO
ALTER TABLE [dbo].[Urunler]  WITH CHECK ADD  CONSTRAINT [FK_Urunler_Birimler] FOREIGN KEY([Stok_birimi])
REFERENCES [dbo].[Birimler] ([Id])
GO
ALTER TABLE [dbo].[Urunler] CHECK CONSTRAINT [FK_Urunler_Birimler]
GO
/****** Object:  StoredProcedure [dbo].[BIRIMSORGULAMA]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[BIRIMSORGULAMA]
	
AS
BEGIN
SET NOCOUNT ON;
	Select Id,Adi from Birimler
	SET NOCOUNT OFF;
END


GO
/****** Object:  StoredProcedure [dbo].[EKLENENURUNUGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EKLENENURUNUGETIR]
	
	@BarkodKodu nvarchar(25)
AS
BEGIN
SET NOCOUNT ON;
Select (Select COUNT(*) From Urunler) AS 'No',Urunler.Bakod_kodu, Urunler.Adi, Urunler.Maliyet ,Urunler.Satis_fiyati, Urunler.Stok, Birimler.Adi,Birimler.Id ,Urunler.Kritik_miktar From Urunler,Birimler Where Birimler.Id=Urunler.Stok_birimi AND Bakod_kodu=@BarkodKodu
	SET NOCOUNt OFF
END


GO
/****** Object:  StoredProcedure [dbo].[ENCOKSATANLAR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[ENCOKSATANLAR]
 @GoruntelenecekAdet int, 
 @Tur tinyint, 
 @BarkodKodu nvarchar(25) 
 AS 
 BEGIN 
	if(@Tur=0)
		Select Top(@GoruntelenecekAdet) ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No',Barkod_Kodu,Urunler.Adi,SUM(Miktar) AS ToplamMiktar, Birimler.Adi from Satis_Detayi,Urunler,Birimler  Where Bakod_kodu=Barkod_Kodu AND Iade=0 AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi Group By Barkod_Kodu,Urunler.Adi,Birimler.Adi Order By (ToplamMiktar) Desc 
	else if(@Tur=1) 
		Select Top(@GoruntelenecekAdet) ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No', Barkod_Kodu,Urunler.Adi,SUM(Miktar) AS ToplamMiktar, Birimler.Adi  from Satis_Detayi,Urunler,Birimler  Where Bakod_kodu=Barkod_Kodu AND Iade=0 AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi Group By Barkod_Kodu,Urunler.Adi,Birimler.Adi Order By ToplamMiktar
	else if (@Tur=2)
		Select ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No', Bakod_kodu,Urunler.Adi, (Select 0) AS ToplamMiktar, Birimler.Adi from Urunler,Birimler Where Bakod_Kodu NOT IN(Select Barkod_Kodu from Satis_Detayi) AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi
	else 
	Select Top(1) ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No', Barkod_Kodu,Urunler.Adi,SUM(Miktar) AS ToplamMiktar, Birimler.Adi  from Satis_Detayi,Urunler,Birimler  Where Bakod_kodu=Barkod_Kodu AND Barkod_Kodu=@BarkodKodu AND Iade=0 AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi Group By Bakod_kodu,Urunler.Adi,Birimler.Adi,Satis_Detayi.Barkod_Kodu Order By Urunler.Adi
END


GO
/****** Object:  StoredProcedure [dbo].[GELIRGIDERGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GELIRGIDERGETIR]
	-- Add the parameters for the stored procedure here
	@BaslangicTarihi datetime,
	@BitisTarihi datetime,
	@SorguTipi tinyint

	--Eğer sorgu tipi 2 ise belirttilen araliktakileri getir,
	--Sorgu tipi eğer 1 ise bir günlük getir,
	--Sorgu tipi eğer 0 ise tümünü getir.
	
AS
BEGIN
	SET NOCOUNT ON;
	 
	IF @SorguTipi=2
	SELECT 
	 ROW_NUMBER() OVER(ORDER BY Gelir_gider.Id DESC) AS 'No',
	 Gelir_gider.Adi, 
	 Gelir_gider.Tarih, 
	 Gelir_gider.Tutar,
	 Gelir_gider.Id
	 From 
	 Gelir_gider
	   Where  Tarih Between @BaslangicTarihi AND DATEADD(DAY,+1,@BitisTarihi)

	ELSE IF @SorguTipi=1
	SELECT 
	 ROW_NUMBER() OVER(ORDER BY Gelir_gider.Id DESC) AS 'No',
	 Gelir_gider.Adi, 
	 Gelir_gider.Tarih, 
	 Gelir_gider.Tutar,
	 Gelir_gider.Id
	 From 
	 Gelir_gider
	   Where  Tarih Between @BaslangicTarihi AND DATEADD(DAY,+1,@BaslangicTarihi)
	ELSE
	SELECT 
	 ROW_NUMBER() OVER(ORDER BY Gelir_gider.Id DESC) AS 'No',
	 Gelir_gider.Adi, 
	 Gelir_gider.Tarih, 
	 Gelir_gider.Tutar,
	 Gelir_gider.Id
	 From 
	 Gelir_gider
	
	SET NOCOUNT OFF
END


GO
/****** Object:  StoredProcedure [dbo].[HIZLIURUNLERIGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[HIZLIURUNLERIGETIR]
AS 
	BEGIN
		SET NOCOUNT ON
				Select Urunler.Bakod_kodu, Urunler.Adi, Urunler.Satis_fiyati, Urunler.Stok, Birimler.Kisaltma,Urunler.Maliyet,Birimler.Id From Urunler JOIN Birimler ON Urunler.Stok_birimi=Birimler.Id Where Hizli_urun = 1
		SET NOCOUNT OFF
	END 


GO
/****** Object:  StoredProcedure [dbo].[ISLEMEKLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ISLEMEKLE]
	
	@Adi nvarchar(100),
	@Tutar decimal(9,2),
	@KasiyerId int

AS
BEGIN
Insert Into Gelir_gider Values(@Adi,GETDATE(),@Tutar)
 Declare @Tarih datetime=(Select MAX(Satislar.Tarih) From Satislar);
	IF(CONVERT(VARCHAR, GETDATE(), 23) = CONVERT(VARCHAR, @Tarih , 23))
	Update Satislar Set KasadakiPara = KasadakiPara+@Tutar Where Satislar.Tarih=@Tarih
	ELSE 
	BEGIN
		Declare @KasadakiToplam decimal(18,2);
		if((Select SUM(Satislar.KasadakiPara) from Satislar) IS NULL)
		Set @KasadakiToplam=0;
		ELSE 
		SET @KasadakiToplam=(Select KasadakiPara from Satislar Where Tarih=(Select MAX(Tarih) from Satislar));
		Insert Into Satislar(Musteri_Id,Tarih,Satis_Bedeli,Satis_Kari,Kasiyer_Id,KasadakiPara,Kredi,Nakit,Taksit) Values ( 0,GETDATE(),0,0,@KasiyerId,@KasadakiToplam+@Tutar,0,0,0)
	END
END


GO
/****** Object:  StoredProcedure [dbo].[ISLEMSIL]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ISLEMSIL]
	-- Add the parameters for the stored procedure here
	@Id int,
	@KasiyerId int,
	@BarkodKodu nvarchar(25),
	@Miktar decimal(9,1)
AS
BEGIN
if(@Miktar>=0)
BEGIN
	Update Satis_Detayi Set Miktar=Miktar-@Miktar Where Barkod_Kodu=@BarkodKodu AND Id=0;
	Update Urunler Set Stok=Stok-@Miktar Where Bakod_kodu=@BarkodKodu;

END
Declare @IslemTutari decimal(9,2) = (Select Tutar From Gelir_gider Where Id=@Id);
Declare @Tarih datetime=(Select MAX(Satislar.Tarih) From Satislar);
	IF(CONVERT(VARCHAR, GETDATE(), 23) = CONVERT(VARCHAR, @Tarih , 23))
	Update Satislar Set KasadakiPara = KasadakiPara-@IslemTutari Where Satislar.Tarih=@Tarih
	ELSE 
	BEGIN
		Declare @KasadakiToplam decimal(18,2);
		if((Select SUM(Satislar.KasadakiPara) from Satislar) IS NULL)
		Set @KasadakiToplam=0;
		ELSE 
		SET @KasadakiToplam=(Select KasadakiPara from Satislar Where Tarih=(Select MAX(Tarih) from Satislar));
		Insert Into Satislar(Musteri_Id,Tarih,Satis_Bedeli,Satis_Kari,Kasiyer_Id,KasadakiPara,Kredi,Taksit,Nakit) Values ( 0,GETDATE(),0,0,@KasiyerId,@KasadakiToplam-@IslemTutari,0,0,0)
	END
	Delete from Gelir_gider Where Id=@Id
END



GO
/****** Object:  StoredProcedure [dbo].[KRITIKURUNSORGULA]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[KRITIKURUNSORGULA]
AS
BEGIN
	SET NOCOUNT ON;
	Select COUNT(Bakod_kodu) AS 'Count' From Urunler Where Kritik_miktar >= Stok AND Bakod_kodu NOT IN(Select Bakod_kodu From Urunler Where Bakod_kodu='0') 
END


GO
/****** Object:  StoredProcedure [dbo].[KULLANICIEKLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Procedure [dbo].[KULLANICIEKLE]
@Adi Nvarchar(50),
@Sifre Nvarchar(50),
@Yetki tinyint
AS
BEGIN
Set NOCOUNT ON
Insert Into Kullanicilar(Adi,Sifre,Yetki) Values(@Adi,@Sifre,@Yetki)
SET NOCOUNT OFF
END


GO
/****** Object:  StoredProcedure [dbo].[KULLANICIGUNCELLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[KULLANICIGUNCELLE]
@Adi Nvarchar(50),
@Sifre Nvarchar(50),
@Yetki tinyint,
@IlkAdi Nvarchar(50)
AS
BEGIN
Set NOCOUNT ON
Select Adi From Kullanicilar Where Adi=@Adi
IF @@ROWCOUNT=1 AND @Adi!=@IlkAdi
	BEGIN
		PRINT 'Aynı';
	END
ELSE 
	BEGIN
		Update Kullanicilar Set Adi=@Adi, Sifre=@Sifre,Yetki=@Yetki Where Adi=@IlkAdi;
	END
SET NOCOUNT OFF
END


GO
/****** Object:  StoredProcedure [dbo].[KULLANICILARIGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[KULLANICILARIGETIR]
AS
BEGIN
Set NOCOUNT ON
Select Adi,Yetki,Sifre From Kullanicilar 
SET NOCOUNT OFF
END


GO
/****** Object:  StoredProcedure [dbo].[KULLANICISIFRESI]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[KULLANICISIFRESI]
@Adi Nvarchar(50),
@EskiSifre Nvarchar(50),
@YeniSifre Nvarchar(50)
AS
BEGIN
Set NOCOUNT ON
Select Sifre From Kullanicilar Where Sifre=@EskiSifre AND Adi=@Adi;
IF @@ROWCOUNT=1
	BEGIN
		Update Kullanicilar Set Sifre=@YeniSifre Where Adi=@Adi;
	END
SET NOCOUNT OFF
END


GO
/****** Object:  StoredProcedure [dbo].[KULLANICISIL]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[KULLANICISIL]
@KullaniciAdi nvarchar(50)
AS
BEGIN
Delete Kullanicilar Where Adi=@KullaniciAdi
END


GO
/****** Object:  StoredProcedure [dbo].[KULLANICISORGULA]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[KULLANICISORGULA]	
	@Adi nvarchar(50),
	@Sifre nvarchar(50)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @Hata AS NVARCHAR(200);
	 Select Adi From Kullanicilar Where Adi=@Adi;
	 IF @@ROWCOUNT=1
		BEGIN
		    Select Id,Adi,Sifre,Yetki FROM Kullanicilar WHERE Sifre=@Sifre AND Adi=@Adi;
			IF @@ROWCOUNT!=1
				BEGIN
				SET @Hata='Sevgili '+@Adi+'. Şifrenizi yanlış girdiniz. Lütfen tekrar deneyin.';
				    SELECT @Hata AS 'Hata';
					PRINT 'Sevgili '+@Adi+'. Şifrenizi yanlış girdiniz. Lütfen tekrar deneyin.';
					RETURN ;
				END
		END
	 ELSE
		BEGIN 
			PRINT @Adi+' adınıda bir kullanıcı bulunamadı. Kullanıcı adınızı doğru girdiğinizden emin olun.';
			RETURN -1;
		END
		SET NOCOUNT OFF;
END


GO
/****** Object:  StoredProcedure [dbo].[MUSTERIEKLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[MUSTERIEKLE]
@Adi nvarchar (50),
@Telefon nvarchar(20),
@AlisverisTutari decimal(9,2),
@Borc decimal(9,2)

AS
BEGIN
Declare @FormatliTelefon varchar(20); 
Set @FormatliTelefon = dbo.PhoneFormat(@Telefon) ;
Declare @SatirSayisi int;
 (Select @SatirSayisi=COUNT(Telefon) From Musteriler Where Telefon=@FormatliTelefon);
IF @SatirSayisi=0
Insert  Into Musteriler(Adi,Telefon,Son_Alisveris_Tarihi,Borc,Toplam_Alisveris) Values(@Adi, @FormatliTelefon, GETDATE(), @Borc, @AlisverisTutari)
ELSE 
UPDATE Musteriler Set Adi=@Adi, Son_Alisveris_Tarihi=GETDATE(),Borc=Borc+@Borc,Toplam_Alisveris=Toplam_Alisveris+@AlisverisTutari Where Telefon=@FormatliTelefon
END


GO
/****** Object:  StoredProcedure [dbo].[MUSTERIGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[MUSTERIGETIR]

AS
BEGIN
SET NOCOUNT ON
Select Adi,Telefon, Son_Alisveris_Tarihi, Borc,Toplam_Alisveris, ROW_NUMBER() OVER(ORDER BY Musteriler.Id) AS 'No' From Musteriler Where Id!=0 And Id!=1
SET NOCOUNT OFF
END


GO
/****** Object:  StoredProcedure [dbo].[MUSTERIGUNCELLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[MUSTERIGUNCELLE]
@IlkAdi nvarchar(50),
@Adi nvarchar(50),
@Telefon nvarchar(25),
@Borc decimal(9,2),
@KasiyerId int
AS
BEGIN
Declare @Odenen decimal(9,2) = (Select Borc From Musteriler Where Telefon=@Telefon)-@Borc;
Declare @Tarih datetime=(Select MAX(Satislar.Tarih) From Satislar);
	IF(CONVERT(VARCHAR, GETDATE(), 23) = CONVERT(VARCHAR, @Tarih , 23))
	Update Satislar Set KasadakiPara = KasadakiPara+@Odenen Where Satislar.Tarih=@Tarih
	ELSE 
	BEGIN
		Declare @KasadakiToplam decimal(18,2);
		if((Select SUM(Satislar.KasadakiPara) from Satislar) IS NULL)
		Set @KasadakiToplam=0;
		ELSE 
		SET @KasadakiToplam=(Select KasadakiPara from Satislar Where Tarih=(Select MAX(Tarih) from Satislar));
		Insert Into Satislar(Musteri_Id,Tarih,Satis_Bedeli,Satis_Kari,Kasiyer_Id,KasadakiPara,Kredi,Taksit,Nakit) Values ( 0,GETDATE(),0,0,@KasiyerId,@KasadakiToplam+@Odenen,0,0,0)
	END
Update Musteriler Set Adi=@Adi, Telefon=@Telefon, Borc=@Borc Where Adi=@IlkAdi
END


GO
/****** Object:  StoredProcedure [dbo].[MUSTERISIL]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[MUSTERISIL]
@Telefon nvarchar(25),
@KasiyerId int

AS
BEGIN
Declare @Odenen decimal(9,2) = (Select Borc From Musteriler Where Telefon=@Telefon);
Declare @Tarih datetime=(Select MAX(Satislar.Tarih) From Satislar);
	IF(CONVERT(VARCHAR, GETDATE(), 23) = CONVERT(VARCHAR, @Tarih , 23))
	Update Satislar Set KasadakiPara = KasadakiPara+@Odenen Where Satislar.Tarih=@Tarih
	ELSE 
	BEGIN
		Declare @KasadakiToplam decimal(18,2);
		if((Select SUM(Satislar.KasadakiPara) from Satislar) IS NULL)
		Set @KasadakiToplam=0;
		ELSE 
		SET @KasadakiToplam=(Select KasadakiPara from Satislar Where Tarih=(Select MAX(Tarih) from Satislar));
		Insert Into Satislar(Musteri_Id,Tarih,Satis_Bedeli,Satis_Kari,Kasiyer_Id,KasadakiPara,Kredi,Taksit,Nakit) Values ( 0,GETDATE(),0,0,@KasiyerId,@KasadakiToplam+@Odenen,0,0,0)
	END
Delete From Musteriler Where Telefon=@Telefon
END


GO
/****** Object:  StoredProcedure [dbo].[OrnekProc]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrnekProc]
@GelenId int,
@Adi nvarchar(11)
AS

	Select Id,Adi from Birimler Where Id=@GelenId AND Adi=@Adi;
GO
/****** Object:  StoredProcedure [dbo].[SATISDETAYIEKLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SATISDETAYIEKLE]
	
	@Id int,
	@BarkodKodu nvarchar(25),
	@Miktar decimal(9,1),
	@Birimi tinyint
AS
BEGIN
	Insert Into Satis_Detayi Values (@Id, @BarkodKodu,@Miktar,@Birimi,0)
END


GO
/****** Object:  StoredProcedure [dbo].[SATISDETAYIGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[SATISDETAYIGETIR]
@SatisId int
AS
BEGIN
SET NOCOUNT ON
SELECT ROW_NUMBER() OVER(ORDER BY Satis_Detayi.Miktar DESC) AS 'No',Satis_Detayi.Barkod_Kodu,Urunler.Adi,Urunler.Satis_fiyati,Satis_Detayi.Miktar,Birimler.Kisaltma, Satis_Detayi.Iade From Satis_Detayi,Birimler,Urunler Where Birimler.Id=Satis_Detayi.BirimId AND Satis_Detayi.Id=@SatisId AND Urunler.Bakod_kodu=Satis_Detayi.Barkod_Kodu
SET NOCOUNT OFF
END


GO
/****** Object:  StoredProcedure [dbo].[SATISEKLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SATISEKLE]
	
	@MusteriId int,
	@Tarih datetime,
	@SatisBedeli decimal(9,2),
	@SatisKari decimal(9,2),
	@Kasiyer_Id int,
	@Borc decimal(9,2),
	@Nakit decimal(9,2),
	@Kredi decimal(9,2),
	@Taksit tinyint 
AS
BEGIN
Declare @KasadakiToplam decimal(18,2);
if((Select SUM(Satislar.KasadakiPara) from Satislar) IS NULL)
Set @KasadakiToplam=0;
ELSE 
SET @KasadakiToplam=(Select KasadakiPara from Satislar Where Tarih=(Select MAX(Tarih) from Satislar));
Declare @KasadakiTutar decimal(18,2)=@KasadakiToplam-@Borc
	Insert Into Satislar Values ( @MusteriId,@Tarih,@SatisBedeli,@SatisKari,@Kasiyer_Id,@KasadakiTutar+@SatisBedeli,@Nakit,@Kredi,@Taksit)
END


GO
/****** Object:  StoredProcedure [dbo].[SATISFILTRELE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SATISFILTRELE]
	-- Add the parameters for the stored procedure here
	@BaslangicTarihi datetime,
	@BitisTarihi datetime,
	@MusteriAdi nvarchar(50),
	@SatisId int,
	@SorguTipi tinyint
	--Eğer sorgu tipi 4 ise müşteriye göre satışları getir,
	--Sorgu tipi eğer 3 ise id'ye eşit satışı getir,
	--Sorgu tipi eğer 2 ise belirttilen araliktaki satışları getir,
	--Sorgu tipi eğer 1 ise bir günlük satışları getir,
	--Sorgu tipi eğer 0 ise tüm satışları getir.
AS
BEGIN
	SET NOCOUNT ON;
	
	 IF @SorguTipi=4
		SELECT s.Id, m.Adi, m.Telefon, s.Tarih, s.Satis_Bedeli,s.Satis_Kari, k.Adi,s.KasadakiPara,s.Nakit, s.Kredi,s.Taksit From Satislar as s INNER JOIN Musteriler m ON s.Musteri_Id=m.Id FULL JOIN Kullanicilar as k ON s.Kasiyer_Id=k.Id WHERE m.Adi LIKE '%'+@MusteriAdi+'%' ORDER BY s.Tarih DESC;
	ELSE IF @SorguTipi=3
	SELECT s.Id, m.Adi, m.Telefon, s.Tarih, s.Satis_Bedeli,s.Satis_Kari, k.Adi,s.KasadakiPara,s.Nakit, s.Kredi,s.Taksit From Satislar as s FULL JOIN Musteriler m ON s.Musteri_Id=m.Id FULL JOIN Kullanicilar as k ON s.Kasiyer_Id=k.Id WHERE s.Id=@SatisId
	ELSE IF @SorguTipi=2
	SELECT s.Id, m.Adi, m.Telefon, s.Tarih, s.Satis_Bedeli,s.Satis_Kari, k.Adi,s.KasadakiPara,s.Nakit, s.Kredi,s.Taksit From Satislar as s FULL JOIN Musteriler m ON s.Musteri_Id=m.Id FULL JOIN Kullanicilar as k ON s.Kasiyer_Id=k.Id  Where s.Tarih Between @BaslangicTarihi AND DATEADD(DAY,+1,@BitisTarihi) ORDER BY s.Tarih DESC

	ELSE IF @SorguTipi=1
	SELECT s.Id, m.Adi, m.Telefon, s.Tarih, s.Satis_Bedeli,s.Satis_Kari, k.Adi,s.KasadakiPara,s.Nakit, s.Kredi,s.Taksit From Satislar as s FULL JOIN Musteriler m ON s.Musteri_Id=m.Id FULL JOIN Kullanicilar as k ON s.Kasiyer_Id=k.Id  Where s.Tarih Between @BaslangicTarihi AND DATEADD(DAY,+1,@BaslangicTarihi) ORDER BY s.Tarih DESC
	ELSE
	SELECT s.Id, m.Adi, m.Telefon, s.Tarih, s.Satis_Bedeli,s.Satis_Kari, k.Adi,s.KasadakiPara,s.Nakit, s.Kredi,s.Taksit From Satislar as s FULL JOIN Musteriler m ON s.Musteri_Id=m.Id FULL JOIN Kullanicilar as k ON s.Kasiyer_Id=k.Id ORDER BY s.Tarih DESC
	Declare @TopIadeEdilen Decimal(9,2)=(Select Sum(sd.Miktar*u.Maliyet) from Satis_Detayi as sd, Urunler as u Where u.Bakod_kodu=sd.Barkod_Kodu AND sd.Id = 0);
	IF(@TopIadeEdilen IS NULL) SET @TopIadeEdilen=0;
	PRINT @TopIadeEdilen;
	IF(@SorguTipi<=2)
	EXEC GELIRGIDERGETIR @BaslangicTarihi,@BitisTarihi,@SorguTipi;
	
	SET NOCOUNT OFF
END

	SELECT s.Id, m.Adi, m.Telefon, s.Tarih, s.Satis_Bedeli,s.Satis_Kari, k.Adi,s.KasadakiPara,s.Nakit, s.Kredi,s.Taksit From Satislar as s FULL JOIN Musteriler m ON s.Musteri_Id=m.Id FULL JOIN Kullanicilar as k ON s.Kasiyer_Id=k.Id WHERE s.Id=@SatisId
GO
/****** Object:  StoredProcedure [dbo].[SONSATISLARIGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SONSATISLARIGETIR]
AS
BEGIN

	SET NOCOUNT ON;
	SELECT TOP 10 ROW_NUMBER() OVER(ORDER BY Satislar.Id DESC) AS 'No',  Musteriler.Adi,Musteriler.Telefon,Satislar.Tarih,Satislar.Satis_Bedeli, Satislar.Id,Kullanicilar.Adi,Satislar.Nakit, Satislar.Kredi, Satislar.Taksit From Musteriler,Satislar,Kullanicilar Where Musteriler.Id=Satislar.Musteri_Id AND Satis_Bedeli>0 AND Kullanicilar.Id=Satislar.Kasiyer_Id;
	SET NOCOUNT OFF;
END


GO
/****** Object:  StoredProcedure [dbo].[URUNEKLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[URUNEKLE] 
	
	@BarkodKodu nvarchar(25),
	@Adi nvarchar(100),
	@Stok decimal(9,1),
	@Maliyet decimal(9,2),
	@SatisFiyati decimal(9,2),
	@StokBirimi tinyint,
	@KritikMiktar decimal(9,1)
AS
BEGIN
	Insert Into Urunler Values (@BarkodKodu, @Adi,@Stok,@Maliyet,@SatisFiyati,@StokBirimi,@KritikMiktar, 0)
END


GO
/****** Object:  StoredProcedure [dbo].[URUNGUNCELLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[URUNGUNCELLE]
	-- Add the parameters for the stored procedure here
	@Adi nvarchar(100),
	@Miktar decimal(9,1),
	@Maliyet decimal(9,2),
	@Fiyat decimal(9,2),
	@KritikMiktar decimal(9,1),
	@Barkod nvarchar(25)

AS
BEGIN
	Update Urunler Set Adi=@Adi, Maliyet=@Maliyet, Stok=@Miktar, Satis_fiyati=@Fiyat, Kritik_miktar=@KritikMiktar Where Bakod_kodu=@Barkod
END


GO
/****** Object:  StoredProcedure [dbo].[URUNIADET]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[URUNIADET]
@BarkodKodu nvarchar(25),
@SatisId int,
@Miktar decimal(9,1)

As
BEGIN 

	If(@BarkodKodu NOT IN (Select Barkod_Kodu From Satis_Detayi,Urunler  Where Barkod_Kodu=@BarkodKodu AND Bakod_kodu=@BarkodKodu))
	BEGIN
		PRINT 'Satılan ürünler arasında '+@BarkodKodu+' barkod kodlu ürün bulunamadı. Silinmiş olabilir. Lütfen doğru ürünü sorguladığınızdan emin olun.';
		RETURN -1;
	END
	DECLARE @Birim nvarchar(25)=(Select Birimler.Adi From Urunler,Birimler Where  Urunler.Bakod_kodu=@BarkodKodu AND Urunler.Stok_birimi=Birimler.Id)
	DECLARE @IadeEdilenMiktar int =(Select Miktar From Satis_Detayi Where Barkod_Kodu=@BarkodKodu AND Id=0)
	if(@IadeEdilenMiktar IS NULL) SET @IadeEdilenMiktar=0;
	DECLARE @SatilanMiktar decimal(9,1)=(Select SUM(Satis_Detayi.Miktar) From Satis_Detayi  Where Barkod_Kodu=@BarkodKodu AND Iade=0);
	
	If(@SatilanMiktar IS NULL) 
	BEGIN
		SET @SatilanMiktar=0;
		IF(@IadeEdilenMiktar>0)
		BEGIN
		PRINT 'Iade etmek istediğiniz, satılan '+Convert(varchar,@IadeEdilenMiktar)+' '+@Birim+' ürünün tamamı zaten iade edilmiş.';
		RETURN -1;
		END
	END
	
	SET @SatilanMiktar=@SatilanMiktar-@IadeEdilenMiktar;
	IF(@Miktar>@SatilanMiktar)
	BEGIN
		PRINT 'İade etmek istediğiniz ürün bu kadar fazla satılmadı. En fazla '+Convert(varchar,@SatilanMiktar)+' '+@Birim+' seçebilirsiniz.';
		RETURN -1;
	END
	if @SatisId=-1 --Satış ıd girilmemişse şunları yap 
	BEGIN 
		Update Urunler Set Stok=Stok+@Miktar Where Bakod_kodu=@BarkodKodu; --Stoğa verline miktar kadar ekle
		
		Declare @UrunFiyati decimal(9,2)=(Select Urunler.Satis_fiyati from Urunler Where Bakod_kodu=@BarkodKodu)*@Miktar;
		Declare @Sonuc decimal(9,2) = (@UrunFiyati*(-1));
		Declare @Tarih datetime=(Select MAX(Satislar.Tarih) From Satislar Where Id!=0);
		DECLARE @IadeEdildiMi int = (Select Count(Id) From Satis_Detayi Where Id=0 AND Barkod_Kodu=@BarkodKodu);
		IF(@IadeEdildiMi>0) Update Satis_Detayi Set Miktar=Miktar+@Miktar Where Id=0 AND Barkod_Kodu=@BarkodKodu --Eğer verilen barkod kodu daha önce 0 id ile kaydedilmişse yeni detay kaydetmeden eskisini gir.
		ELSE INSERT INTO Satis_Detayi Values(0,@BarkodKodu,@Miktar,Convert(int,(Select Urunler.Stok_birimi From Urunler Where Bakod_kodu=@BarkodKodu)),1) --Eğer verilen barkod kodu daha önce 0 id ile kaydedilmemişse 0 id ile barkod kodunu kaydet.
	
		PRINT Convert(varchar,@Sonuc);
	END
	ELSE 
	BEGIN

		If(@SatisId NOT IN (Select Id From Satislar Where Id=@SatisId))
		BEGIN
			PRINT Convert(varchar,@SatisId)+' numaralı satış bulunamadı. Lütfen satış numarasını doğru girdiğinizden emin olun.';
		RETURN -1;
		END

		If(@BarkodKodu NOT IN (Select Barkod_kodu From Satis_Detayi  Where Barkod_kodu=@BarkodKodu AND Id=@SatisId))
		BEGIN
			PRINT Convert(varchar,@SatisId)+' numaralı satışta '+@BarkodKodu+' kodlu ürün satılmadı. Lütfen doğru ürünü sorguladığınızdan emin olun.';
		RETURN -1;
		END

		IF (@Miktar>(Select Satis_Detayi.Miktar From Satis_Detayi Where Satis_Detayi.Id=@SatisId AND Satis_Detayi.Barkod_Kodu=@BarkodKodu))
		BEGIN
			PRINT Convert(varchar,@SatisId)+' numaralı satışta bu üründen bu kadar ürün satılmadı. Bu ürün daha önce iade edilmiş olabilir. Satış işlemlerinden satış numarasını girerek detayına bakabilirsiniz.';
			RETURN -1;
		END
			
			Declare @fiyat decimal(9,2)=(Select Urunler.Satis_fiyati From Urunler Where Bakod_kodu=@BarkodKodu);
			Declare @kar decimal(9,2)=(Select Satis_fiyati-Maliyet From Urunler Where Bakod_kodu=@BarkodKodu);
			Declare @miktarliFiyat decimal(9,2)=@fiyat*@Miktar;
			Declare @Result nvarchar(200)='Ürün iade işlemi başarılı bir şekilde gerçekleşti. ';
		if((Select Id From Musteriler Where Id=(Select Satislar.Musteri_Id From Satislar Where Id=@SatisId))!=0 AND (Select Id From Musteriler Where Id=(Select Satislar.Musteri_Id From Satislar Where Id=@SatisId))!=1 )
		BEGIN
			Declare @borc decimal(9,2)=(Select Borc From Musteriler Where Id=(Select Satislar.Musteri_Id From Satislar Where Id=@SatisId));
			IF(@borc<@miktarliFiyat)
			BEGIN
				
				
				Set @Result='Ürün iade işlemi başarılı bir şekilde gerçekleşti. '+Convert(nvarchar,(Select Telefon From Musteriler Where Id=(Select Satislar.Musteri_Id From Satislar Where Id=@SatisId)))+' iletişim numaralı müşterinin borcu tamamen silindi. Müşteriye '+Convert(nvarchar,(@miktarliFiyat-@borc))+' TL geri vermeniz gerek.';
				Set @borc=0;

			END
			ELSE IF (@borc=@miktarliFiyat)
			BEGIN
				Set @borc=0;
				Set @Result='Ürün iade işlemi başarılı bir şekilde gerçekleşti. '+Convert(nvarchar,(Select Telefon From Musteriler Where Id=(Select Satislar.Musteri_Id From Satislar Where Id=@SatisId)))+' iletişim numaralı müşterinin borcu tamamen silindi.';
			END
			ELSE
			BEGIN
			Set @borc=@borc-@miktarliFiyat;
				Set @Result='Ürün iade işlemi başarılı bir şekilde gerçekleşti. '+Convert(nvarchar,(Select Telefon From Musteriler Where Id=(Select Satislar.Musteri_Id From Satislar Where Id=@SatisId)))+' iletişim numaralı müşterinin borcundan '+Convert(nvarchar,@miktarliFiyat)+' TL silindi.'
			END
			Update Musteriler Set Toplam_Alisveris=Toplam_Alisveris-@miktarliFiyat, Borc=@borc Where Id=(Select Satislar.Musteri_Id From Satislar Where Id=@SatisId)

		END
		
		Update Satislar Set Satis_Bedeli=Satis_Bedeli-@miktarliFiyat, Satis_Kari=Satis_Kari-(@kar*@Miktar) Where Satislar.Id=@SatisId;

		Update Satislar Set KasadakiPara=KasadakiPara-@miktarliFiyat Where Tarih>=(Select Tarih From Satislar Where Id=@SatisId)
		Update Satis_Detayi Set Miktar=Miktar-@Miktar, Iade=1 Where Barkod_kodu=@BarkodKodu AND Satis_Detayi.Id=@SatisId;
		Update Urunler Set Stok=Stok+@Miktar Where Bakod_kodu=@BarkodKodu;
		PRINT @Result
	END		
END


GO
/****** Object:  StoredProcedure [dbo].[URUNLERIGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[URUNLERIGETIR]
	@Kritik bit
AS
BEGIN
SET NOCOUNT ON
IF @Kritik=0
	Select ROW_NUMBER() OVER(ORDER BY Urunler.Stok)  AS 'No',Urunler.Bakod_kodu, Urunler.Adi, Urunler.Maliyet ,Urunler.Satis_fiyati, Urunler.Stok, Birimler.Adi,Birimler.Id ,Urunler.Kritik_miktar From Urunler,Birimler Where Birimler.Id=Urunler.Stok_birimi AND Urunler.Bakod_kodu NOT IN (Select Urunler.Bakod_kodu From Urunler Where Bakod_kodu='0')
ELSE
Select ROW_NUMBER() OVER(ORDER BY Urunler.Stok)  AS 'No',Urunler.Bakod_kodu, Urunler.Adi, Urunler.Maliyet ,Urunler.Satis_fiyati, Urunler.Stok, Birimler.Adi,Birimler.Id ,Urunler.Kritik_miktar From Urunler,Birimler Where Birimler.Id=Urunler.Stok_birimi  AND Urunler.Stok<=Urunler.Kritik_miktar AND Urunler.Bakod_kodu NOT IN (Select Urunler.Bakod_kodu From Urunler Where Bakod_kodu='0')
	SET NOCOUNt OFF
END


GO
/****** Object:  StoredProcedure [dbo].[URUNSIL]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[URUNSIL]
	-- Add the parameters for the stored procedure here
	@Barkod varchar(25)

AS
BEGIN
	DELETE FROM Urunler Where Bakod_kodu=@Barkod
END


GO
/****** Object:  StoredProcedure [dbo].[URUNSORGULAMA]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[URUNSORGULAMA]
@BarkodKodu Nvarchar(25)
AS 
	BEGIN
		SET NOCOUNT ON
				Select Urunler.Bakod_kodu, Urunler.Adi, Urunler.Satis_fiyati, Urunler.Stok, Birimler.Kisaltma,Urunler.Maliyet,Birimler.Id From Urunler,Birimler Where Bakod_kodu=@BarkodKodu AND Birimler.Id=Urunler.Stok_birimi
		SET NOCOUNT OFF
	END 


GO
/****** Object:  Trigger [dbo].[MUSTERIIDSINIGETIR]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER  [dbo].[MUSTERIIDSINIGETIR] ON  [dbo].[Musteriler] AFTER INSERT,UPDATE AS BEGIN SET NOCOUNT ON;Select CAST((Select Id from Inserted)AS nvarchar(4))+'_'+Adi From Inserted;SET NOCOUNT OFF;END

GO
ALTER TABLE [dbo].[Musteriler] ENABLE TRIGGER [MUSTERIIDSINIGETIR]
GO
/****** Object:  Trigger [dbo].[MUSTERISILINIRKEN]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE Trigger [dbo].[MUSTERISILINIRKEN] on [dbo].[Musteriler]
Instead Of Delete
AS
BEGIN
Update Satislar Set Musteri_Id=1 Where Musteri_Id in (Select Id from deleted );
Delete from Musteriler Where Id in (Select Id from deleted )
END

GO
ALTER TABLE [dbo].[Musteriler] ENABLE TRIGGER [MUSTERISILINIRKEN]
GO
/****** Object:  Trigger [dbo].[DETAYEKLE]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE Trigger [dbo].[DETAYEKLE] on [dbo].[Satis_Detayi]
After Insert
AS
BEGIN
Declare @Barkod nvarchar(25);
Select @Barkod=Barkod_Kodu from inserted;
Update Urunler Set Stok=Stok-(Select Miktar from inserted) where Bakod_kodu = @Barkod;
END

GO
ALTER TABLE [dbo].[Satis_Detayi] ENABLE TRIGGER [DETAYEKLE]
GO
/****** Object:  Trigger [dbo].[SonIdGetir]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[SonIdGetir]
   ON [dbo].[Satislar]
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;
	SELECT Id from inserted
    SET NOCOUNT OFF;

END

GO
ALTER TABLE [dbo].[Satislar] ENABLE TRIGGER [SonIdGetir]
GO
/****** Object:  Trigger [dbo].[URUNSILINIRKEN]    Script Date: 31.10.2020 05:47:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[URUNSILINIRKEN]
   ON  [dbo].[Urunler] 
   INSTEAD OF DELETE
AS 
BEGIN
	SET NOCOUNT ON;
	Update Satis_Detayi Set Barkod_Kodu=0 Where Barkod_Kodu IN (Select Bakod_kodu From deleted);
	Delete FROM Urunler Where Bakod_kodu IN (Select Bakod_kodu From deleted);
	SET NOCOUNT OFF
END

GO
ALTER TABLE [dbo].[Urunler] ENABLE TRIGGER [URUNSILINIRKEN]
GO
USE [master]
GO
ALTER DATABASE [OtomasyonDB] SET  READ_WRITE 
GO
