import React, { useEffect, useState } from "react";
import { Table, Input, Select, Button, Tag } from "antd";
import { SearchOutlined, DollarOutlined } from "@ant-design/icons";
import { usePositions } from "@/features/positions/hooks/usePositions";
import { PositionDto } from "@/features/positions/types/position.types";
import { PositionSalaryStepDrawer } from "./PositionSalaryStepDrawer";
import "./P1SalaryManagement.css";

export const P1SalaryManagement: React.FC = () => {
  const { positions, loading, fetchPositions } = usePositions();
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string | undefined>(
    undefined,
  );
  const [isSalaryDrawerOpen, setIsSalaryDrawerOpen] = useState(false);
  const [selectedPosForSalary, setSelectedPosForSalary] = useState<
    string | null
  >(null);
  const [selectedPosNameForSalary, setSelectedPosNameForSalary] =
    useState<string>("");

  useEffect(() => {
    fetchPositions(searchTerm, statusFilter);
  }, [searchTerm, statusFilter, fetchPositions]);

  const handleOpenSalaryDrawer = (record: PositionDto) => {
    setSelectedPosForSalary(record.idChucVu);
    setSelectedPosNameForSalary(record.tenChucVu);
    setIsSalaryDrawerOpen(true);
  };

  const columns = [
    {
      title: "Mã Chức Vụ",
      dataIndex: "idChucVu",
      key: "idChucVu",
      width: 150,
      render: (text: string) => (
        <span className="font-mono text-gray-600 dark:text-gray-400">
          {text}
        </span>
      ),
    },
    {
      title: "Tên Chức Vụ",
      dataIndex: "tenChucVu",
      key: "tenChucVu",
      render: (text: string) => (
        <span className="font-semibold text-gray-800 dark:text-gray-100">
          {text}
        </span>
      ),
    },
    {
      title: "Trạng Thái",
      dataIndex: "trangThai",
      key: "trangThai",
      width: 150,
      align: "center" as const,
      render: (status: string) =>
        status === "HOAT_DONG" ? (
          <Tag color="success" className="rounded-full px-3">
            Đang hoạt động
          </Tag>
        ) : (
          <Tag color="default" className="rounded-full px-3 text-gray-500">
            Ngừng HĐ
          </Tag>
        ),
    },
    {
      title: "Hành Động",
      key: "actions",
      width: 200,
      align: "right" as const,
      render: (_: unknown, record: PositionDto) => (
        <Button
          type="primary"
          ghost
          icon={<DollarOutlined />}
          onClick={() => handleOpenSalaryDrawer(record)}
          className="border-violet-500 text-violet-600 hover:bg-violet-50 dark:hover:bg-violet-900/30"
        >
          Cấu hình lương
        </Button>
      ),
    },
  ];

  return (
    <div className="p1-salary-wrapper min-w-0 overflow-hidden flex flex-col h-full">
      <div className="p1-salary-header flex-shrink-0">
        <div>
          <h2 className="p1-salary-title">Quản Lý Lương Theo Chức Vụ</h2>
          <p className="text-sm text-gray-500 mt-1">
            Thiết lập và quản lý các bậc lương cơ bản (P1) theo từng chức vụ
          </p>
        </div>
      </div>

      <div className="p1-salary-card flex flex-col flex-1 min-w-0 min-h-0 overflow-hidden">
        <div className="p1-salary-toolbar flex-shrink-0">
          <Input
            placeholder="Tìm theo Mã, Tên chức vụ..."
            prefix={<SearchOutlined className="text-gray-400" />}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            allowClear
            className="w-full sm:max-w-md h-10 rounded-lg"
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
            scroll={{ x: 800 }}
            pagination={{
              pageSize: 10,
              className:
                "px-4 py-3 m-0 border-t border-gray-100 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50",
            }}
          />
        </div>
      </div>

      <PositionSalaryStepDrawer
        isOpen={isSalaryDrawerOpen}
        onClose={() => setIsSalaryDrawerOpen(false)}
        positionId={selectedPosForSalary}
        positionName={selectedPosNameForSalary}
      />
    </div>
  );
};
