export interface PhanCongCaDto {
    idPhanCong: string;
    cccdNhanVien: string;
    ngayLamViec: string; // "YYYY-MM-DD"
    idCaLamViec: string;
    tenCa: string;
    hoTenNhanVien: string;
    ghiChu?: string;
}

export interface UpsertPhanCongCaCommand {
    cccdNhanVien: string;
    ngayLamViec: string; // "YYYY-MM-DD"
    idCaLamViec: string | null;
    ghiChu?: string;
}
