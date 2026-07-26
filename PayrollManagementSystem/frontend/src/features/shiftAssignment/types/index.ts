export interface PhanCongCaDto {
    idPhanCong: string;
    cccdNhanVien: string;
    ngayLamViec: string; // "YYYY-MM-DD"
    idCaLamViec: string | null;
    tenCa: string | null;
    hoTenNhanVien: string;
    ghiChu?: string;
}

export interface UpsertPhanCongCaCommand {
    cccdNhanVien: string;
    ngayLamViec: string; // "YYYY-MM-DD"
    idCaLamViec: string | null;
    xoaPhanCong?: boolean;
    ghiChu?: string;
}
