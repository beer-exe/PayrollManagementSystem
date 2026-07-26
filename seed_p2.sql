DELETE FROM khung_nang_luc_p2;
INSERT INTO khung_nang_luc_p2 (id_tieu_chi, id_chuc_vu, ten_nang_luc, mo_ta, ty_trong, "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted")
VALUES
-- CV_HRM (Trưởng Phòng Nhân Sự)
(gen_random_uuid(), 'CV_HRM', 'Kỹ năng quản trị nhân sự', 'Kiến thức và kỹ năng về quản trị, tuyển dụng, đào tạo', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_HRM', 'Kỹ năng lãnh đạo', 'Khả năng quản lý và dẫn dắt đội ngũ nhân sự', 30, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_HRM', 'Kỹ năng giao tiếp và giải quyết xung đột', 'Xử lý các vấn đề nội bộ hiệu quả', 30, NOW(), NULL, NULL, NULL, false),

-- CV_ITM (Trưởng Phòng IT)
(gen_random_uuid(), 'CV_ITM', 'Năng lực chuyên môn IT', 'Kiến thức về hệ thống, bảo mật và phát triển phần mềm', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_ITM', 'Kỹ năng quản lý dự án', 'Lên kế hoạch và điều phối các dự án công nghệ', 30, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_ITM', 'Kỹ năng lãnh đạo', 'Quản lý, phân công công việc cho đội ngũ kỹ thuật', 30, NOW(), NULL, NULL, NULL, false),

-- CV_DEV (Lập Trình Viên)
(gen_random_uuid(), 'CV_DEV', 'Kỹ năng lập trình', 'Thành thạo ngôn ngữ lập trình, framework và công cụ phát triển', 50, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_DEV', 'Kỹ năng giải quyết vấn đề', 'Khả năng phân tích và xử lý lỗi phần mềm (debug, fix bug)', 30, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_DEV', 'Kỹ năng làm việc nhóm', 'Phối hợp tốt với team và tuân thủ quy trình Agile/Scrum', 20, NOW(), NULL, NULL, NULL, false),

-- CV_INTERN_DEV (Lập trình viên thực tập)
(gen_random_uuid(), 'CV_INTERN_DEV', 'Kiến thức nền tảng', 'Hiểu biết cơ bản về lập trình và cấu trúc dữ liệu', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_INTERN_DEV', 'Tinh thần học hỏi', 'Chủ động tiếp thu kiến thức và kỹ năng mới', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_INTERN_DEV', 'Sự tuân thủ và kỷ luật', 'Thực hiện đúng hướng dẫn và quy định của công ty', 20, NOW(), NULL, NULL, NULL, false),

-- CV_HRS (Nhân Viên Nhân Sự)
(gen_random_uuid(), 'CV_HRS', 'Kỹ năng tuyển dụng', 'Thực hiện quy trình tuyển dụng và phỏng vấn sơ bộ', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_HRS', 'Nghiệp vụ tính lương & BHXH', 'Thực hiện tính lương, chế độ bảo hiểm chính xác', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_HRS', 'Kỹ năng giao tiếp', 'Hỗ trợ và tư vấn cho nhân viên trong công ty', 20, NOW(), NULL, NULL, NULL, false),

-- CV_CEO (Giám Đốc Sản Xuất)
(gen_random_uuid(), 'CV_CEO', 'Hoạch định chiến lược sản xuất', 'Xây dựng kế hoạch và tầm nhìn sản xuất dài hạn', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_CEO', 'Quản lý vận hành', 'Đảm bảo hoạt động sản xuất diễn ra hiệu quả và an toàn', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_CEO', 'Ra quyết định', 'Quyết đoán trong xử lý các vấn đề lớn của bộ phận', 20, NOW(), NULL, NULL, NULL, false),

-- CV_CEO2 (Giám Đốc Kinh Doanh)
(gen_random_uuid(), 'CV_CEO2', 'Tư duy chiến lược kinh doanh', 'Hoạch định chiến lược kinh doanh, phát triển thị trường', 40, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_CEO2', 'Kỹ năng đàm phán', 'Khả năng thương lượng và chốt các hợp đồng lớn', 30, NOW(), NULL, NULL, NULL, false),
(gen_random_uuid(), 'CV_CEO2', 'Quản trị rủi ro', 'Đánh giá và giảm thiểu rủi ro trong kinh doanh', 30, NOW(), NULL, NULL, NULL, false);
