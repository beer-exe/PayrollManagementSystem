export interface KhungNangLucDto {
  idTieuChi: string;
  idChucVu: string;
  tenNangLuc: string;
  moTa?: string;
  tyTrong: number;
}

export interface CreateKhungNangLucCommand {
  idChucVu: string;
  tenNangLuc: string;
  moTa?: string;
  tyTrong: number;
}

export interface UpdateKhungNangLucCommand {
  idTieuChi: string;
  tenNangLuc: string;
  moTa?: string;
  tyTrong: number;
}
