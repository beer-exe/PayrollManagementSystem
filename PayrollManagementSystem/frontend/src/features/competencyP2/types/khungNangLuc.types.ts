export interface KhungNangLucDto {
  idTieuChi: string;
  idChucVu: string;
  tenNangLuc: string;
  yeuCauToiThieu: string;
  tyTrong: number;
}

export interface CreateKhungNangLucCommand {
  idChucVu: string;
  tenNangLuc: string;
  yeuCauToiThieu: string;
  tyTrong: number;
}

export interface UpdateKhungNangLucCommand {
  idTieuChi: string;
  tenNangLuc: string;
  yeuCauToiThieu: string;
  tyTrong: number;
}
