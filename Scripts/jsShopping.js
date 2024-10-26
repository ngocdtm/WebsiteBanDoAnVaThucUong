$(document).ready(function () {
    ShowCount();
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        console.log('AddToCart button clicked'); // Add this line
        var id = $(this).data('id');
        //var storeId = $(this).data('storeid');
        var storeId = 1;


        var quantity = 1;
        var tQuantity = $('#quantity_value').text();
        var selectedToppingIds = [];
        if (tQuantity != '') {
            quantity = parseInt(tQuantity);
        }
        var toppings = []; // Khởi tạo mảng toppings
        $('input[name="selectedToppingIds"]:checked').each(function () {
            toppings.push($(this).val()); // Thêm topping được chọn vào mảng
        });
        var Size = $('#selectedSizeId option:selected').text();  // Giả sử bạn có dropdown hoặc input với ID là 'sizeSelector'

        if (Size === undefined || Size === "" || Size === null) {
            Size = "S";
        }

        if (selectedToppingIds === undefined || selectedToppingIds.length === 0) {
            selectedToppingIds = [];
        } else {
            selectedToppingIds = toppings
        }


        // Kiểm tra nếu storeId không tồn tại
        if (!storeId) {
            alert("Không tìm thấy storeId.");
            return;
        }

        $.ajax({
            url: '/shoppingCart/AddToCart',
            type: 'POST',
            data: {
                id: id, quantity: quantity, selectedSizeId: Size,
                selectedToppingIds: toppings, storeId: storeId
            },
            success: function (rs) {
                if (rs.Success) {
                    $('#checkout_items').html(rs.Count);
                    toastr.success(rs.msg); // Hiển thị thông báo thành công
                } else {
                  
                    alert("Có lỗi xảy ra: " + rs.msg);
                }
            },
             
            error: function (xhr, status, error) {
                console.log("ID: " + id);
                console.log("Quantity: " + quantity);
                console.log("Selected Size ID: " + Size);
                console.log("Selected Topping IDs: " + toppings);
                console.log("Store ID: " + storeId);

                console.error("AJAX Error: " + status + error);
                alert("Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng");
            }
        });
    });
    $('body').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        var quantity = $('#Quantity_' + id).val();
        var storeId = $(this).data('storeid') || $('#store_id_hidden').val(); // Thử lấy từ nút bấm hoặc input hidden

        // Kiểm tra nếu storeId không tồn tại
        if (!storeId) {
            alert("Không tìm thấy storeId.");
            return;
        }

        Update(id, quantity, storeId);
    });

    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm('Bạn có chắc muốn xóa hết sản phẩm trong giỏ hàng?');
        //debugger;
        if (conf == true) {
            DeleteAll();
        }

    });

    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var storeId = $(this).data('storeid') || $('#store_id_hidden').val(); // Thử lấy từ nút bấm hoặc input hidden
        var conf = confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?');

        if (conf == true) {
            if (!storeId) {
                alert("Không tìm thấy storeId.");
                return;
            }
            $.ajax({
                url: '/shoppingcart/Delete',
                type: 'POST',
                data: { id: id, storeId: storeId },
                success: function (rs) {
                    if (rs.Success) {
                        $('#checkout_items').html(rs.Count);
                        $('#trow_' + id).remove();
                        LoadCart();  // Reload giỏ hàng sau khi xóa sản phẩm
                    }
                },
                error: function (xhr, status, error) {
                    console.error("AJAX Error: Status = " + status + ", Error = " + error);
                    alert("Có lỗi xảy ra khi xóa sản phẩm khỏi giỏ hàng.");
                }
            });
        }
    });

});



function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}
function DeleteAll() {
    $.ajax({
        url: '/shoppingcart/DeleteAll',
        type: 'POST',
        success: function (rs) {
            if (rs.Success) {
                LoadCart();  // Reload giỏ hàng sau khi xóa tất cả sản phẩm
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error: Status = " + status + ", Error = " + error);
            alert("Có lỗi xảy ra khi xóa toàn bộ sản phẩm khỏi giỏ hàng.");
        }
    });
}

function Update(id, quantity, storeId) {
    $.ajax({
        url: '/shoppingCart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity, storeId: storeId },
        success: function (rs) {
            if (rs.Success) {
                LoadCart();  // Reload giỏ hàng sau khi cập nhật
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error: Status = " + status + ", Error = " + error);
            console.error("Response Text: " + xhr.responseText);
            alert("Có lỗi xảy ra khi cập nhật giỏ hàng: " + xhr.responseText);
        }
    });
}


function LoadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);  // Cập nhật nội dung giỏ hàng vào phần tử với id 'load_data'
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error: Status = " + status + ", Error = " + error);
            alert("Có lỗi xảy ra khi tải lại giỏ hàng.");
        }
    });
}
