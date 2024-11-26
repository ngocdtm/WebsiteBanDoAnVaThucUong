- Các chức năng: Chuyển khoản trực tuyến(VNPAY), Chat realtime, Tính phí ship theo tọa độ từ cửa hàng đến khách hàng, Tích điểm hội viên, Cửa hàng chọn thành phố, huyện, xã theo web Pronvince API web ,Mua hàng trên từng chi nhánh kèm các món ăn tự chọn (topping,size, extra), các danh sách chương trình khuyến mãi(combo 2 tặng 1, 20% trên 1 sản phẩm, 15% trên 1 sản phẩm), tìm kiếm sản phẩm và hiển thị sản phẩm gợi ý, lịch sử các sản phẩm đã xem, mua hàng COD, xem các sản phẩm liên quan, chatbot, đăng review(rating, comment), khóa tài khoản khách hàng, quản lý thống kê trên các chi nhánh và top các sản phẩm bán, Popup ads, Quản lý banner quảng cáo và còn nhiều chức năng khác kèm từng bước chi tiết về hình ảnh của website (vào link google doc sau: https://docs.google.com/document/d/1D5mRUX6kwJzETZFjGVG6X0E_kbbzjk2H-4iktD5gXlA/edit?usp=sharing)

- Vào xem web hosting demo sau: https://websitebandoanvathucuong.azurewebsites.net/

- Tổng quan trang user: ![image](https://github.com/user-attachments/assets/736fd90b-6dd3-44da-ad91-3d56531ad49d)

- Tổng quan trang admin: ![image](https://github.com/user-attachments/assets/aa94f261-98dd-45d8-8b64-758eee17976f)

- Công nghệ sử dụng: Asp.net c# trên visual studio và hệ quản trị: sql server
  
* Hướng dẫn Clone project
1) Copy web URL của project
![image](https://github.com/user-attachments/assets/0bcbbdc7-e6eb-41be-9f82-2872cba981c7)

2) Mở Visual Studio, trên thanh menu, Chọn Git > Clone Repository
![image](https://github.com/user-attachments/assets/57b9d6a8-9f20-4669-b464-d4ef802a59d1)

3) Dán link URL vào, chọn location bạn muốn lưu loacal project. Sau đó nhấn Clone
![image](https://github.com/user-attachments/assets/e768b6d3-f6d6-4e4f-87fa-d28d0ebafde7)

4) Vào file web.config, ở đường dẫn connectionString, hãy thay đổi thành Data Source của bạn (là Server Name trong SQL Server)
![image](https://github.com/user-attachments/assets/8f275563-32d2-42f1-8bf7-ab23b0caaaff)
![image](https://github.com/user-attachments/assets/5ce8883b-cc00-4dab-a382-c12854a7f6f1)

6) Sau đó chạy file script để lấy được dữ liệu
7) Run Project
