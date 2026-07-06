CREATE EXTENSION IF NOT EXISTS pgcrypto;


CREATE TABLE chuc_vus (
    id_chuc_vu character varying(50) NOT NULL,
    ten_chuc_vu character varying(100) NOT NULL,
    mo_ta_cong_viec character varying(500),
    "TrangThai" integer NOT NULL,
    CONSTRAINT chuc_vus_pkey PRIMARY KEY (id_chuc_vu)
);


CREATE TABLE ky_danh_gias (
    id_ky_danh_gia uuid NOT NULL DEFAULT (gen_random_uuid()),
    ten_ky_danh_gia character varying(200) NOT NULL,
    nam integer NOT NULL,
    ngay_bat_dau date NOT NULL,
    ngay_ket_thuc date NOT NULL,
    trang_thai character varying(50) NOT NULL,
    CONSTRAINT ky_danh_gias_pkey PRIMARY KEY (id_ky_danh_gia)
);


CREATE TABLE moi_quan_hes (
    id_mqh uuid NOT NULL DEFAULT (gen_random_uuid()),
    ten_quan_he character varying(100) NOT NULL,
    CONSTRAINT moi_quan_hes_pkey PRIMARY KEY (id_mqh)
);


CREATE TABLE muc_quy_doi_p2s (
    id_quy_doi uuid NOT NULL DEFAULT (gen_random_uuid()),
    xep_loai character varying(100) NOT NULL,
    diem_toi_thieu numeric(5,2) NOT NULL,
    diem_toi_da numeric(5,2) NOT NULL,
    he_so_p2 numeric(5,2) NOT NULL,
    CONSTRAINT muc_quy_doi_p2s_pkey PRIMARY KEY (id_quy_doi)
);


CREATE TABLE phong_bans (
    id_pb character varying(50) NOT NULL,
    ten_pb character varying(100) NOT NULL,
    CONSTRAINT phong_bans_pkey PRIMARY KEY (id_pb)
);


CREATE TABLE than_nhans (
    ma_dinh_danh character varying(50) NOT NULL,
    ten_tn character varying(150) NOT NULL,
    ngay_sinh date,
    CONSTRAINT than_nhans_pkey PRIMARY KEY (ma_dinh_danh)
);


CREATE TABLE vai_tros (
    id_vai_tro uuid NOT NULL,
    ten_vai_tro character varying(100) NOT NULL,
    CONSTRAINT vai_tros_pkey PRIMARY KEY (id_vai_tro)
);


CREATE TABLE bac_luongs (
    id_bac_luong character varying(50) NOT NULL,
    id_chuc_vu character varying(50) NOT NULL,
    ten_bac_luong character varying(100) NOT NULL,
    luong_p1 numeric(18,2) NOT NULL,
    ngay_ap_dung date NOT NULL,
    ngay_ket_thuc date,
    "TrangThai" integer NOT NULL,
    CONSTRAINT bac_luongs_pkey PRIMARY KEY (id_bac_luong),
    CONSTRAINT bac_luongs_id_chuc_vu_fkey FOREIGN KEY (id_chuc_vu) REFERENCES chuc_vus (id_chuc_vu) ON DELETE CASCADE
);


CREATE TABLE khung_nang_luc_p2 (
    id_tieu_chi uuid NOT NULL DEFAULT (gen_random_uuid()),
    id_chuc_vu character varying(50) NOT NULL,
    ten_nang_luc character varying(150) NOT NULL,
    yeu_cau_toi_thieu character varying(500) NOT NULL,
    ty_trong numeric(5,2) NOT NULL,
    CONSTRAINT khung_nang_luc_p2_pkey PRIMARY KEY (id_tieu_chi),
    CONSTRAINT khung_nang_luc_id_chuc_vu_fkey FOREIGN KEY (id_chuc_vu) REFERENCES chuc_vus (id_chuc_vu) ON DELETE CASCADE
);


CREATE TABLE tai_khoans (
    id_tai_khoan uuid NOT NULL,
    ten_tai_khoan character varying(50) NOT NULL,
    mat_khau_hash character varying(255) NOT NULL,
    trang_thai character varying(50) NOT NULL,
    dang_nhap_lan_dau boolean DEFAULT TRUE,
    id_vai_tro uuid,
    refresh_token text,
    refresh_token_expiry_time timestamp without time zone,
    CONSTRAINT tai_khoans_pkey PRIMARY KEY (id_tai_khoan),
    CONSTRAINT tai_khoans_id_vai_tro_fkey FOREIGN KEY (id_vai_tro) REFERENCES vai_tros (id_vai_tro) ON DELETE SET NULL
);


CREATE TABLE nhan_viens (
    cccd character varying(20) NOT NULL,
    ho_ten character varying(150) NOT NULL,
    gioi_tinh boolean,
    sdt character varying(15),
    email character varying(100),
    ngay_sinh date,
    dan_toc character varying(50),
    dia_chi character varying(255),
    chuyen_nganh character varying(100),
    ngay_vao_lam date,
    ngay_nghi_viec date,
    trang_thai character varying(50),
    so_bhxh character varying(50),
    so_bhyt character varying(50),
    so_tai_khoan character varying(50),
    ten_ngan_hang character varying(100),
    ma_so_thue character varying(50),
    id_pb character varying(50),
    id_tai_khoan uuid,
    cccd_nguoi_quan_ly character varying(20),
    CONSTRAINT nhan_viens_pkey PRIMARY KEY (cccd),
    CONSTRAINT nhan_viens_cccd_nguoi_quan_ly_fkey FOREIGN KEY (cccd_nguoi_quan_ly) REFERENCES nhan_viens (cccd) ON DELETE SET NULL,
    CONSTRAINT nhan_viens_id_pb_fkey FOREIGN KEY (id_pb) REFERENCES phong_bans (id_pb) ON DELETE SET NULL,
    CONSTRAINT nhan_viens_id_tai_khoan_fkey FOREIGN KEY (id_tai_khoan) REFERENCES tai_khoans (id_tai_khoan) ON DELETE SET NULL
);


CREATE TABLE hop_dong_lao_dongs (
    so_hop_dong character varying(50) NOT NULL,
    cccd character varying(20) NOT NULL,
    loai_hop_dong character varying(100) NOT NULL,
    ngay_bat_dau date NOT NULL,
    ngay_ket_thuc date,
    luong_co_ban numeric(18,2) NOT NULL,
    trang_thai character varying(50) NOT NULL,
    CONSTRAINT hop_dong_lao_dongs_pkey PRIMARY KEY (so_hop_dong),
    CONSTRAINT hop_dong_lao_dongs_cccd_fkey FOREIGN KEY (cccd) REFERENCES nhan_viens (cccd) ON DELETE CASCADE
);


CREATE TABLE nhat_ky_trang_thais (
    id_nhat_ky uuid NOT NULL DEFAULT (gen_random_uuid()),
    cccd character varying(20) NOT NULL,
    trang_thai_cu character varying(50),
    trang_thai_moi character varying(50) NOT NULL,
    ly_do character varying(255) NOT NULL,
    ngay_thay_doi timestamp without time zone NOT NULL,
    nguoi_thay_doi character varying(150) NOT NULL,
    CONSTRAINT nhat_ky_trang_thais_pkey PRIMARY KEY (id_nhat_ky),
    CONSTRAINT nhat_ky_trang_thais_cccd_fkey FOREIGN KEY (cccd) REFERENCES nhan_viens (cccd) ON DELETE CASCADE
);


CREATE TABLE phieu_danh_gia_nang_lucs (
    id_phieu uuid NOT NULL DEFAULT (gen_random_uuid()),
    id_ky_danh_gia uuid NOT NULL,
    cccd_nhan_vien character varying(20) NOT NULL,
    cccd_quan_ly character varying(20),
    diem_tong_hop numeric(5,2),
    he_so_p2 numeric(5,2),
    xep_loai character varying(100),
    nhan_xet_chung character varying(1000),
    trang_thai character varying(50) NOT NULL,
    CONSTRAINT phieu_danh_gia_nang_lucs_pkey PRIMARY KEY (id_phieu),
    CONSTRAINT phieu_danh_gias_cccd_nhan_vien_fkey FOREIGN KEY (cccd_nhan_vien) REFERENCES nhan_viens (cccd) ON DELETE CASCADE,
    CONSTRAINT phieu_danh_gias_cccd_quan_ly_fkey FOREIGN KEY (cccd_quan_ly) REFERENCES nhan_viens (cccd) ON DELETE SET NULL,
    CONSTRAINT phieu_danh_gias_id_ky_danh_gia_fkey FOREIGN KEY (id_ky_danh_gia) REFERENCES ky_danh_gias (id_ky_danh_gia) ON DELETE CASCADE
);


CREATE TABLE quyet_dinh_nhan_sus (
    so_quyet_dinh character varying(50) NOT NULL,
    cccd character varying(20),
    loai_quyet_dinh character varying(100) NOT NULL,
    id_bac_luong_moi character varying(50),
    id_chuc_vu_moi character varying(50),
    ngay_hieu_luc date NOT NULL,
    ngay_het_han date,
    nguoi_ky character varying(100),
    trang_thai character varying(50) NOT NULL,
    CONSTRAINT quyet_dinh_nhan_sus_pkey PRIMARY KEY (so_quyet_dinh),
    CONSTRAINT quyet_dinh_nhan_sus_cccd_fkey FOREIGN KEY (cccd) REFERENCES nhan_viens (cccd) ON DELETE CASCADE,
    CONSTRAINT quyet_dinh_nhan_sus_id_bac_luong_moi_fkey FOREIGN KEY (id_bac_luong_moi) REFERENCES bac_luongs (id_bac_luong) ON DELETE SET NULL
);


CREATE TABLE than_nhan_nhan_vien (
    cccd character varying(20) NOT NULL,
    ma_dinh_danh character varying(50) NOT NULL,
    id_mqh uuid,
    CONSTRAINT than_nhan_nhan_vien_pkey PRIMARY KEY (cccd, ma_dinh_danh),
    CONSTRAINT than_nhan_nhan_vien_cccd_fkey FOREIGN KEY (cccd) REFERENCES nhan_viens (cccd) ON DELETE CASCADE,
    CONSTRAINT than_nhan_nhan_vien_id_mqh_fkey FOREIGN KEY (id_mqh) REFERENCES moi_quan_hes (id_mqh) ON DELETE SET NULL,
    CONSTRAINT than_nhan_nhan_vien_ma_dinh_danh_fkey FOREIGN KEY (ma_dinh_danh) REFERENCES than_nhans (ma_dinh_danh) ON DELETE CASCADE
);


CREATE TABLE chi_tiet_danh_gia_nang_lucs (
    id_chi_tiet uuid NOT NULL DEFAULT (gen_random_uuid()),
    id_phieu uuid NOT NULL,
    id_tieu_chi uuid NOT NULL,
    diem_tu_danh_gia integer,
    diem_quan_ly_danh_gia integer,
    nhan_xet_nhan_vien character varying(500),
    nhan_xet_quan_ly character varying(500),
    CONSTRAINT chi_tiet_danh_gia_nang_lucs_pkey PRIMARY KEY (id_chi_tiet),
    CONSTRAINT chi_tiet_danh_gias_id_phieu_fkey FOREIGN KEY (id_phieu) REFERENCES phieu_danh_gia_nang_lucs (id_phieu) ON DELETE CASCADE,
    CONSTRAINT chi_tiet_danh_gias_id_tieu_chi_fkey FOREIGN KEY (id_tieu_chi) REFERENCES khung_nang_luc_p2 (id_tieu_chi) ON DELETE CASCADE
);


CREATE INDEX "IX_bac_luongs_id_chuc_vu" ON bac_luongs (id_chuc_vu);


CREATE INDEX "IX_chi_tiet_danh_gia_nang_lucs_id_phieu" ON chi_tiet_danh_gia_nang_lucs (id_phieu);


CREATE INDEX "IX_chi_tiet_danh_gia_nang_lucs_id_tieu_chi" ON chi_tiet_danh_gia_nang_lucs (id_tieu_chi);


CREATE INDEX "IX_hop_dong_lao_dongs_cccd" ON hop_dong_lao_dongs (cccd);


CREATE INDEX "IX_khung_nang_luc_p2_id_chuc_vu" ON khung_nang_luc_p2 (id_chuc_vu);


CREATE INDEX "IX_nhan_viens_cccd_nguoi_quan_ly" ON nhan_viens (cccd_nguoi_quan_ly);


CREATE INDEX "IX_nhan_viens_id_pb" ON nhan_viens (id_pb);


CREATE UNIQUE INDEX "IX_nhan_viens_id_tai_khoan" ON nhan_viens (id_tai_khoan);


CREATE UNIQUE INDEX nhan_viens_email_key ON nhan_viens (email);


CREATE INDEX "IX_nhat_ky_trang_thais_cccd" ON nhat_ky_trang_thais (cccd);


CREATE INDEX "IX_phieu_danh_gia_nang_lucs_cccd_nhan_vien" ON phieu_danh_gia_nang_lucs (cccd_nhan_vien);


CREATE INDEX "IX_phieu_danh_gia_nang_lucs_cccd_quan_ly" ON phieu_danh_gia_nang_lucs (cccd_quan_ly);


CREATE INDEX "IX_phieu_danh_gia_nang_lucs_id_ky_danh_gia" ON phieu_danh_gia_nang_lucs (id_ky_danh_gia);


CREATE INDEX "IX_quyet_dinh_nhan_sus_cccd" ON quyet_dinh_nhan_sus (cccd);


CREATE INDEX "IX_quyet_dinh_nhan_sus_id_bac_luong_moi" ON quyet_dinh_nhan_sus (id_bac_luong_moi);


CREATE INDEX "IX_tai_khoans_id_vai_tro" ON tai_khoans (id_vai_tro);


CREATE UNIQUE INDEX tai_khoans_ten_tai_khoan_key ON tai_khoans (ten_tai_khoan);


CREATE INDEX "IX_than_nhan_nhan_vien_id_mqh" ON than_nhan_nhan_vien (id_mqh);


CREATE INDEX "IX_than_nhan_nhan_vien_ma_dinh_danh" ON than_nhan_nhan_vien (ma_dinh_danh);


