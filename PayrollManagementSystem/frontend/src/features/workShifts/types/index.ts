export interface KhungGioNghi {
    id?: string;
    idCaLamViec?: string;
    tenKhoangNghi: string;
    gioBatDau: string; // "HH:mm:ss"
    gioKetThuc: string; // "HH:mm:ss"
    tinhVaoGioLam: boolean;
}

export interface CaLamViec {
    id: string;
    tenCa: string;
    gioBatDau: string; // "HH:mm:ss"
    gioKetThuc: string; // "HH:mm:ss"
    xuyenNgay: boolean;
    heSoLuong: number;
    trangThai: boolean;
    khungGioNghis: KhungGioNghi[];
}

export interface CreateCaLamViecRequest {
    tenCa: string;
    gioBatDau: string;
    gioKetThuc: string;
    xuyenNgay: boolean;
    heSoLuong: number;
    trangThai: boolean;
    khungGioNghis: Omit<KhungGioNghi, 'id' | 'idCaLamViec'>[];
}

export interface UpdateCaLamViecRequest extends CreateCaLamViecRequest {
    id: string;
    khungGioNghis: KhungGioNghi[];
}
