import React, { useEffect } from 'react';
import { Card, Table, Tag, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';

export const DuyetDanhGia: React.FC = () => {
  const { data, loading, fetchManagerEvaluations } = usePhieuDanhGia();
  const navigate = useNavigate();

  useEffect(() => {
    fetchManagerEvaluations();
  }, [fetchManagerEvaluations]);

  const columns = [
    { title: 'Kỳ đánh giá', dataIndex: 'tenKyDanhGia', key: 'tenKyDanhGia' },
    { title: 'CCCD Nhân viên', dataIndex: 'cccdNhanVien', key: 'cccdNhanVien' },
    { 
      title: 'Trạng thái', 
      dataIndex: 'trangThai', 
      key: 'trangThai',
      render: (status: string) => {
        if (status === 'CHO_QL_DANH_GIA') return <Tag color="blue">Chờ duyệt</Tag>;
        if (status === 'DA_HOAN_THANH') return <Tag color="green">Đã hoàn thành</Tag>;
        return <Tag>{status}</Tag>;
      }
    },
    {
      title: 'Điểm tổng hợp',
      dataIndex: 'diemTongHop',
      key: 'diemTongHop',
      render: (diem: number | null) => diem !== null ? diem : '-'
    },
    {
      title: 'Xếp loại',
      dataIndex: 'xepLoai',
      key: 'xepLoai',
      render: (xepLoai: string | null) => xepLoai || '-'
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: unknown, record: any) => (
        <Button 
          type={record.canEvaluate && record.trangThai === 'CHO_QL_DANH_GIA' ? 'primary' : 'default'}
          onClick={() => navigate(`/dashboard/danh-gia/duyet-danh-gia/${record.idPhieu}`)}
        >
          {record.canEvaluate && record.trangThai === 'CHO_QL_DANH_GIA' ? 'Chấm điểm' : 'Xem chi tiết'}
        </Button>
      )
    }
  ];

  return (
    <Card title="Danh sách & Duyệt đánh giá">
      <Table 
        columns={columns} 
        dataSource={data} 
        rowKey="idPhieu" 
        loading={loading}
      />
    </Card>
  );
};
