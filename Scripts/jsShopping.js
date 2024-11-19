$(document).ready(function () {
    ShowCount();
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        console.log('AddToCart button clicked'); // Add this line
        var id = $(this).data('id');
        var storeId = $(this).data('storeid');
        var quantity = 1;
        var tQuantity = $('#quantity_value').text();
        // Collect selected topping, size, and extra IDs
        var toppingIds = [];
        $('.topping-option:checked').each(function () {
            toppingIds.push($(this).val());
            console.log("Selected topping id:", $(this).val());
        });

        var sizeId = null;
        var selectedSize = $('.size-option:checked');
        if (selectedSize.length > 0) {
            sizeId = selectedSize.val();
            console.log("Selected size id:", sizeId);
        }

        // Get selected extra IDs
        var extraIds = [];
        $('.extra-option:checked').each(function () {
            extraIds.push(parseInt($(this).val()));
            console.log("Selected extra id:", $(this).val());
        });

        console.log('Selected options:', {
            id: id,
            storeId: storeId,
            quantity: quantity,
            sizeId: sizeId,
            toppingIds: toppingIds,
            extraIds: extraIds
        });

        if (tQuantity != '') {
            quantity = parseInt(tQuantity);
        }
        // Kiểm tra nếu storeId không tồn tại
        if (!storeId) {
            alert("Không tìm thấy storeId.");
            return;
        }
        $.ajax({
            url: '/shoppingCart/AddToCart',
            type: 'POST',
            traditional: true, // Quan trọng khi gửi mảng
            data: {
                id: id, quantity: quantity, storeId: storeId,
                sizeId: sizeId,
                toppingIds: toppingIds,
                extraIds: extraIds },
            success: function (rs) {
                if (rs.Success) {
                    $('#checkout_items').html(rs.Count);
                    alert(rs.msg);
                } else {
                    alert("Có lỗi xảy ra: " + rs.msg);
                }
            },
            error: function (xhr, status, error) {
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
        // Lấy thông tin về size, toppings và extras từ item
        var sizeIds = $(this).data('sizeids') ? $(this).data('sizeids').toString().split(',').map(Number) : [];
        var toppingIds = $(this).data('toppingids') ? $(this).data('toppingids').toString().split(',').map(Number) : [];
        var extraIds = $(this).data('extraids') ? $(this).data('extraids').toString().split(',').map(Number) : [];

        // Kiểm tra nếu storeId không tồn tại
        if (!storeId) {
            alert("Không tìm thấy storeId.");
            return;
        }

        Update(id, quantity, storeId, sizeIds, toppingIds, extraIds);

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
       
        // Lấy thông tin về size, toppings và extras từ item
        var sizeIds = $(this).data('sizeids') ? $(this).data('sizeids').toString().split(',').map(Number) : [];
        var toppingIds = $(this).data('toppingids') ? $(this).data('toppingids').toString().split(',').map(Number) : [];
        var extraIds = $(this).data('extraids') ? $(this).data('extraids').toString().split(',').map(Number) : [];
        var conf = confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?');

        if (conf == true) {
            if (!storeId) {
                alert("Không tìm thấy storeId.");
                return;
            }
            $.ajax({
                url: '/shoppingcart/Delete',
                type: 'POST',
                data: {
                    id: id,
                    storeId: storeId,
                    sizeId: sizeIds && sizeIds.length > 0 ? sizeIds[0] : null,
                    toppingIds: toppingIds || [],
                    extraIds: extraIds || []
                },
                traditional: true,
                success: function (rs) {
                    if (rs.Success) {
                        $('#checkout_items').html(rs.Count);
                        $('#trow_' + id).remove();
                        LoadCart();  // Reload giỏ hàng sau khi xóa sản phẩm
                    } else {
                        alert(rs.msg);
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

function Update(id, quantity, storeId, sizeIds, toppingIds, extraIds) {
    $.ajax({
        url: '/shoppingCart/Update',
        type: 'POST',
        data: {
            id: id,
            quantity: quantity,
            storeId: storeId,
            sizeId: sizeIds && sizeIds.length > 0 ? sizeIds[0] : null,
            toppingIds: toppingIds || [],
            extraIds: extraIds || []
        },
        traditional: true,
        success: function (rs) {
            if (rs.Success) {
                LoadCart();  // Reload giỏ hàng sau khi cập nhật
                ShowCount(); // Cập nhật số lượng items trong giỏ hàng
            } else {
                alert(rs.msg);
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
