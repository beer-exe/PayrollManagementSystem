import React, { useEffect } from 'react';
import { Card, Table, Button, Tag, Space, Modal } from 'antd';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';
import { useKyDanhGia } from '../hooks/useKyDanhGia';
import { useNavigate } from 'react-router-dom';

export const TuDanhGia: React.FC = () => {
  const { data: myForms, loading: formLoading, fetchMyEvaluations, generate } = usePhieuDanhGia();
  const { data: kyDanhGias, loading: kyLoading, fetchKyDanhGia } = useKyDanhGia();
  const navigate = useNavigate();

  useEffect(() => {
    fetchMyEvaluations();
    fetchKyDanhGia();
  }, [fetchMyEvaluations, fetchKyDanhGia]);

  const handleGenerate = (idKyDanhGia: string) => {
    Modal.confirm({
      title: 'Tạo phiếu tự đánh giá',
      content: 'Bạn có chắc muốn tự động tạo phiếu đánh giá cho kỳ này không? Các tiêu chí sẽ được load tự động dựa vào chức vụ hiện tại của bạn.',
      onOk: async () => {
        const success = await generate({ idKyDanhGia });
        if (success) {
          fetchMyEvaluations();
        }
      }
    });
  };

  const activeKys = kyDanhGias.filter(x => x.trangThai === 'DANG_DANH_GIA');

  const columns = [
    { title: 'Kỳ đánh giá', dataIndex: 'tenKyDanhGia', key: 'tenKyDanhGia' },
    { title: 'Điểm tổng hợp', dataIndex: 'diemTongHop', key: 'diemTongHop', render: (val: any) => val ?? '-' },
    { title: 'Hệ số P2', dataIndex: 'heSoP2', key: 'heSoP2', render: (val: any) => val ?? '-' },
    { title: 'Xếp loại', dataIndex: 'xepLoai', key: 'xepLoai', render: (val: any) => val ?? '-' },
    { 
      title: 'Trạng thái', 
      dataIndex: 'trangThai', 
      key: 'trangThai',
      render: (tt: string, record: any) => {
        let color = 'default';
        if (tt === 'CHO_NV_DANH_GIA') color = 'blue';
        if (tt === 'CHO_QL_DANH_GIA') color = 'orange';
        if (tt === 'DA_HOAN_THANH') color = 'green';
        return <Tag color={color}>{record.tenTrangThai || tt}</Tag>;
      }
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: unknown, record: any) => (
        <Button 
          type="primary" 
          onClick={() => navigate(`/dashboard/danh-gia/tu-danh-gia/${record.idPhieu}`)}
        >
          {record.trangThai === 'CHO_NV_DANH_GIA' ? 'Làm phiếu' : 'Xem chi tiết'}
        </Button>
      )
    }
  ];

  return (
    <Space direction="vertical" className="w-full">
      <Card title="Các kỳ đánh giá đang mở">
        {activeKys.length === 0 ? (
          <p>Không có kỳ đánh giá nào đang mở.</p>
        ) : (
          <Space direction="vertical">
            {activeKys.map(ky => {
              const hasForm = myForms.find(f => f.idKyDanhGia === ky.idKyDanhGia);
              return (
                <div key={ky.idKyDanhGia} className="flex justify-between items-center p-4 border dark:border-gray-700 rounded-lg bg-gray-50 dark:bg-gray-800">
                  <div>
                    <strong>{ky.tenKyDanhGia}</strong> ({ky.ngayBatDau} - {ky.ngayKetThuc})
                  </div>
                  {!hasForm ? (
                    <Button type="primary" onClick={() => handleGenerate(ky.idKyDanhGia)}>
                      Tạo phiếu đánh giá
                    </Button>
                  ) : (
                    <Tag color="green">Đã có phiếu</Tag>
                  )}
                </div>
              );
            })}
          </Space>
        )}
      </Card>

      <Card title="Danh sách phiếu đánh giá của tôi">
        <Table 
          columns={columns} 
          dataSource={myForms} 
          rowKey="idPhieu" 
          loading={formLoading || kyLoading} 
          scroll={{ x: 'max-content' }}
        />
      </Card>
    </Space>
  );
};
