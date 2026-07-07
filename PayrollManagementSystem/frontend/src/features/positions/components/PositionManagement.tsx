import React, { useEffect, useState } from "react";
import {
  Table,
  Input,
  Select,
  Button,
  Modal,
  Form,
  Tag,
  Space,
  Checkbox,
} from "antd";
import {
  PlusOutlined,
  SearchOutlined,
  EditOutlined,
  RetweetOutlined,
} from "@ant-design/icons";
import { usePositions } from "../hooks/usePositions";
import { PositionDto } from "../types/position.types";
import { useJobGrades } from "../../jobGrades/hooks/useJobGrades";
import { departmentApi } from "../../departments/api/departmentApi";
import { DepartmentDto } from "../../departments/types/department.types";

export const PositionManagement: React.FC = () => {
  const {
    positions,
    loading,
    fetchPositions,
    createPosition,
    updatePosition,
    toggleStatus,
  } = usePositions();

  const { jobGrades, fetchJobGrades: fetchJobGradesData } = useJobGrades();

  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string | undefined>(
    undefined,
  );
  const [selectedDepartmentId, setSelectedDepartmentId] = useState<string | undefined>(
    undefined,
  );
  
  const [departments, setDepartments] = useState<DepartmentDto[]>([]);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingPos, setEditingPos] = useState<PositionDto | null>(null);
  const [hasManager, setHasManager] = useState(false);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchPositions(searchTerm, statusFilter, selectedDepartmentId);
  }, [searchTerm, statusFilter, selectedDepartmentId, fetchPositions]);

  useEffect(() => {
    fetchJobGradesData();
    departmentApi.getDepartments().then(res => {
      if (res.succeeded) setDepartments(res.data);
    });
  }, [fetchJobGradesData]);

  const handleOpenModal = (record?: PositionDto) => {
    setEditingPos(record || null);
    if (record) {
      form.setFieldsValue(record);
      setHasManager(!!record.idChucVuQuanLy);
    } else {
      form.resetFields();
      setHasManager(false);
      if (selectedDepartmentId) {
        form.setFieldsValue({ idPhongBan: selectedDepartmentId });
      }
    }
    setIsModalOpen(true);
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      let success = false;
      if (editingPos) {
        success = await updatePosition(editingPos.idChucVu, {
          ...values,
          idChucVu: editingPos.idChucVu,
        });
      } else {
        success = await createPosition(values);
      }

      if (success) {
        setIsModalOpen(false);
        fetchPositions(searchTerm, statusFilter, selectedDepartmentId);
      }
    } catch (info) {
      console.log("Validate Failed:", info);
    }
  };

  const handleToggleStatus = (record: PositionDto) => {
    Modal.confirm({
      title: "Xác nhận chuyển trạng thái",
      content:
        record.trangThai === "HOAT_DONG"
          ? `Bạn có chắc muốn vô hiệu hóa chức vụ "${record.tenChucVu}"?`
          : `Bạn có muốn kích hoạt lại chức vụ "${record.tenChucVu}"?`,
      okText: "Đồng ý",
      cancelText: "Hủy",
      onOk: async () => {
        const success = await toggleStatus(record.idChucVu);
        if (success) fetchPositions(searchTerm, statusFilter, selectedDepartmentId);
      },
    });
  };

  const columns = [
    {
      title: "Mã Chức Vụ",
      dataIndex: "idChucVu",
      key: "idChucVu",
      width: 150,
      render: (text: string) => <span className="font-mono">{text}</span>,
    },
    {
      title: "Tên Chức Vụ",
      dataIndex: "tenChucVu",
      key: "tenChucVu",
      render: (text: string) => (
        <span className="font-semibold text-gray-800 dark:text-white">
          {text}
        </span>
      ),
    },
    {
      title: "Phòng Ban",
      dataIndex: "tenPhongBan",
      key: "tenPhongBan",
      render: (text: string) => (
        <span className="text-gray-600 dark:text-gray-300">
          {text || "Chưa gán"}
        </span>
      ),
    },
    {
      title: "Quản Lý Trực Tiếp",
      dataIndex: "tenChucVuQuanLy",
      key: "tenChucVuQuanLy",
      render: (text: string) => (
        <span className="text-gray-600 dark:text-gray-300">
          {text || "-"}
        </span>
      ),
    },
    {
      title: "Mô Tả Công Việc",
      dataIndex: "moTaCongViec",
      key: "moTaCongViec",
      ellipsis: true,
    },
    {
      title: "Ngạch Lương",
      dataIndex: "tenNgachLuong",
      key: "tenNgachLuong",
      render: (text: string) => text ? <Tag color="blue">{text}</Tag> : <span className="text-gray-400">Chưa gán</span>
    },
    {
      title: "Trạng Thái",
      dataIndex: "trangThai",
      key: "trangThai",
      width: 150,
      align: "center" as const,
      render: (status: string, record: PositionDto) =>
        status === "HOAT_DONG" ? (
          <Tag color="success" className="rounded-full px-3">
            {record.tenTrangThai}
          </Tag>
        ) : (
          <Tag color="default" className="rounded-full px-3 text-gray-500">
            {record.tenTrangThai}
          </Tag>
        ),
    },
    {
      title: "Hành Động",
      key: "actions",
      width: 300,
      align: "right" as const,
      render: (_: unknown, record: PositionDto) => (
        <Space size="small">

          <Button
            type="text"
            icon={<EditOutlined />}
            className="text-violet-600 hover:bg-violet-50"
            onClick={() => handleOpenModal(record)}
          >
            Sửa
          </Button>

          <Button
            type="text"
            icon={<RetweetOutlined />}
            className={
              record.trangThai === "HOAT_DONG"
                ? "text-amber-600 hover:bg-amber-50"
                : "text-emerald-600 hover:bg-emerald-50"
            }
            onClick={() => handleToggleStatus(record)}
          >
            {record.trangThai === "HOAT_DONG" ? "Vô hiệu hóa" : "Kích hoạt"}
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div className="p-4 sm:p-6 h-full flex flex-col bg-gray-50/50 dark:bg-gray-900 min-w-0 overflow-hidden">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
        <div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
            Danh Mục Chức Vụ
          </h2>
          <p className="text-sm text-gray-500 mt-1">
            Quản lý các chức vụ và mô tả công việc (3P)
          </p>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleOpenModal()}
          disabled={!selectedDepartmentId}
          className="bg-violet-600 hover:bg-violet-700 h-10 px-5 rounded-lg shadow-sm"
        >
          Thêm Chức Vụ
        </Button>
      </div>

      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 flex flex-col flex-1 overflow-hidden min-h-0">
        <div className="flex flex-col sm:flex-row p-4 border-b border-gray-100 gap-4 flex-shrink-0">
          <Input
            placeholder="Tìm theo Mã, Tên chức vụ..."
            prefix={<SearchOutlined className="text-gray-400" />}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full sm:max-w-xs h-10 rounded-lg"
          />
          <Select
            placeholder="Lọc theo phòng ban"
            allowClear
            value={selectedDepartmentId}
            onChange={setSelectedDepartmentId}
            className="w-full sm:w-64 h-10"
            options={departments?.map((d: any) => ({ label: d.tenPb, value: d.idPb }))}
          />
          <Select
            placeholder="Lọc theo trạng thái"
            allowClear
            value={statusFilter}
            onChange={setStatusFilter}
            className="w-full sm:w-48 h-10"
            options={[
              { value: "HOAT_DONG", label: "Đang hoạt động" },
              { value: "NGUNG_HOAT_DONG", label: "Ngừng hoạt động" },
            ]}
          />
        </div>

        <div className="grid grid-cols-1 w-full flex-1 min-h-0">
          <Table
            dataSource={positions}
            columns={columns}
            rowKey="idChucVu"
            loading={loading}
            scroll={{ x: 1050 }}
            pagination={{
              pageSize: 10,
              className: "px-4 py-3 m-0 border-t border-gray-100 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50",
            }}
          />
        </div>
      </div>

      <Modal
        title={
          <h3 className="text-xl font-bold text-gray-900 pb-2 border-b border-gray-100 mb-4">
            {editingPos ? "Cập Nhật Chức Vụ" : "Thêm Mới Chức Vụ"}
          </h3>
        }
        open={isModalOpen}
        onOk={handleSubmit}
        onCancel={() => setIsModalOpen(false)}
        okText="Lưu lại"
        cancelText="Hủy bỏ"
        destroyOnClose
        okButtonProps={{
          className: "bg-violet-600 hover:bg-violet-700 rounded-lg",
        }}
        cancelButtonProps={{ className: "rounded-lg" }}
      >
        <Form form={form} layout="vertical" className="mt-2">
          <Form.Item
            name="idChucVu"
            label={
              <span className="font-semibold text-gray-700">Mã Chức Vụ</span>
            }
            rules={[{ required: true, message: "Vui lòng nhập mã chức vụ!" }]}
          >
            <Input
              placeholder="VD: CV_TRUONGPHONG"
              size="large"
              disabled={!!editingPos}
              className="rounded-lg"
            />
          </Form.Item>
          <Form.Item
            name="tenChucVu"
            label={
              <span className="font-semibold text-gray-700">Tên Chức Vụ</span>
            }
            rules={[{ required: true, message: "Vui lòng nhập tên chức vụ!" }]}
          >
            <Input
              placeholder="VD: Trưởng Phòng"
              size="large"
              className="rounded-lg"
            />
          </Form.Item>
          <Form.Item
            name="moTaCongViec"
            label={
              <span className="font-semibold text-gray-700">
                Mô Tả Công Việc
              </span>
            }
          >
            <Input.TextArea
              placeholder="Mô tả sơ lược về công việc, trách nhiệm..."
              rows={4}
              className="rounded-lg"
            />
          </Form.Item>
          <Form.Item
            name="idNgachLuong"
            label={
              <span className="font-semibold text-gray-700">Ngạch Lương</span>
            }
          >
            <Select
              placeholder="Chọn ngạch lương"
              allowClear
              size="large"
              options={jobGrades?.filter((g: any) => g.trangThai === 1).map((g: any) => ({ label: g.tenNgachLuong, value: g.idNgachLuong }))}
            />
          </Form.Item>
          
          <Form.Item
            name="idPhongBan"
            label={
              <span className="font-semibold text-gray-700">Phòng Ban</span>
            }
            rules={[{ required: true, message: "Vui lòng chọn phòng ban!" }]}
          >
            <Select
              placeholder="Chọn phòng ban"
              allowClear
              size="large"
              disabled={true}
              options={departments?.map((d: any) => ({ label: d.tenPb, value: d.idPb }))}
            />
          </Form.Item>

          <Form.Item className="mb-2">
            <Checkbox
              checked={hasManager}
              onChange={(e) => {
                setHasManager(e.target.checked);
                if (!e.target.checked) {
                  form.setFieldsValue({ idChucVuQuanLy: undefined });
                }
              }}
            >
              <span className="font-medium text-gray-700">Chức vụ này có báo cáo cho Quản lý trực tiếp?</span>
            </Checkbox>
          </Form.Item>

          {hasManager && (
            <Form.Item
              name="idChucVuQuanLy"
              label={
                <span className="font-semibold text-gray-700">Quản Lý Trực Tiếp (Báo cáo cho)</span>
              }
              rules={[{ required: true, message: "Vui lòng chọn chức vụ quản lý trực tiếp!" }]}
            >
              <Select
                placeholder="Chọn chức vụ quản lý"
                allowClear
                size="large"
                options={positions?.filter(p => p.idChucVu !== editingPos?.idChucVu && p.trangThai === "HOAT_DONG").map((p: any) => ({ label: `${p.tenChucVu} - ${p.tenPhongBan}`, value: p.idChucVu }))}
              />
            </Form.Item>
          )}
        </Form>
      </Modal>
    </div>
  );
};
