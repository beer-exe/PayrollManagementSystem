import React, { useEffect, useState } from "react";
import { Modal, Form, Select, DatePicker, Input, message, Alert } from "antd";
import { departmentApi } from "../../api/departmentApi";
import { EmployeeInDepartmentDto } from "../../types/department.types";
import { PositionDto } from "@/features/positions/types/position.types";
import { salaryStepApi } from "@/features/salarySteps/api/salaryStepApi";
import { SalaryStepDto } from "@/features/salarySteps/types/salaryStep.types";

interface AdjustSalaryModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  employee: EmployeeInDepartmentDto | null;
  positions: PositionDto[];
}

export const AdjustSalaryModal: React.FC<AdjustSalaryModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  employee,
  positions,
}) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const [salarySteps, setSalarySteps] = useState<SalaryStepDto[]>([]);
  const [loadingSteps, setLoadingSteps] = useState(false);

  const fetchSalarySteps = async (jobGradeId: string) => {
    setLoadingSteps(true);
    try {
      const res = await salaryStepApi.getActive(jobGradeId);
      if (res.succeeded) {
        setSalarySteps(res.data);
      }
    } catch (error) {
      message.error("Lỗi tải danh sách bậc lương");
    } finally {
      setLoadingSteps(false);
    }
  };

  useEffect(() => {
    if (isOpen && employee) {
      form.resetFields();

      const currentPos = positions.find(
        (p) => p.tenChucVu === employee.tenChucVu,
      );
      const jobGradeId = currentPos?.idNgachLuong;

      if (jobGradeId) {
        fetchSalarySteps(jobGradeId);
      } else {
        setSalarySteps([]);
        message.warning(
          "Không xác định được ngạch lương của chức vụ hiện tại.",
        );
      }
    }
  }, [isOpen, employee, positions, form]);

  const handleSubmit = async () => {
    if (!employee) return;

    try {
      const values = await form.validateFields();
      setLoading(true);

      const payload = {
        soQuyetDinh: values.soQuyetDinh,
        cccd: employee.cccd,
        idBacLuongMoi: values.idBacLuongMoi,
        ngayHieuLuc: values.ngayHieuLuc.format("YYYY-MM-DD"),
        lyDo: values.lyDo,
      };

      const res = await departmentApi.adjustSalary(payload);
      if (res.succeeded) {
        message.success("Điều chỉnh bậc lương thành công!");
        onSuccess();
        onClose();
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(
        err.response?.data?.Message ||
          "Lỗi khi lưu quyết định điều chỉnh lương",
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title="Quyết Định Điều Chỉnh Bậc Lương"
      open={isOpen}
      onOk={handleSubmit}
      onCancel={onClose}
      confirmLoading={loading}
      okText="Xác nhận"
      cancelText="Hủy bỏ"
      destroyOnClose
      okButtonProps={{ className: "bg-emerald-600 hover:bg-emerald-700" }}
    >
      {employee && (
        <Alert
          message={
            <span className="font-semibold text-emerald-800">
              Nhân sự: {employee.hoTen}
            </span>
          }
          description={`Mã NV (CCCD): ${employee.cccd} - Chức vụ hiện hành: ${employee.tenChucVu}`}
          type="success"
          showIcon
          className="mb-6 bg-emerald-50 border-emerald-200"
        />
      )}

      <Form form={form} layout="vertical">
        <Form.Item
          name="soQuyetDinh"
          label="Số quyết định"
          rules={[{ required: true, message: "Vui lòng nhập số quyết định!" }]}
        >
          <Input placeholder="VD: 125/QĐ-LƯƠNG" />
        </Form.Item>

        <Form.Item
          name="idBacLuongMoi"
          label="Chọn Bậc Lương Mới (P1)"
          rules={[{ required: true, message: "Vui lòng chọn bậc lương" }]}
        >
          <Select
            placeholder="-- Chọn bậc lương áp dụng --"
            loading={loadingSteps}
            disabled={salarySteps.length === 0}
            options={salarySteps.map((s) => ({
              label: `${s.stepName} - ${s.p1Salary.toLocaleString("vi-VN")} VNĐ`,
              value: s.id,
            }))}
            notFoundContent="Chức vụ này hiện chưa cấu hình danh sách bậc lương"
          />
        </Form.Item>

        <Form.Item
          name="ngayHieuLuc"
          label="Ngày hiệu lực"
          rules={[{ required: true, message: "Vui lòng chọn ngày hiệu lực" }]}
        >
          <DatePicker format="DD/MM/YYYY" className="w-full" />
        </Form.Item>

        <Form.Item
          name="lyDo"
          label="Lý do điều chỉnh (Tăng lương định kỳ, đột xuất,...)"
        >
          <Input.TextArea rows={3} placeholder="Ghi chú lý do..." />
        </Form.Item>
      </Form>
    </Modal>
  );
};
