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
        Update(id, quantity);

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
        var conf = confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?');
        if (conf == true) {
            $.ajax({
                url: '/shoppingcart/Delete',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    if (rs.Success) {
                        $('#checkout_items').html(rs.Count);
                        $('#trow_' + id).remove();
                        LoadCart();
                    }
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
                LoadCart();
            }
        }
    });
}
function Update(id, quantity) {
    $.ajax({
        url: '/shoppingcart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (rs) {
            if (rs.Success) {
                LoadCart();
            }
        }
    });
}

function LoadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }
    });
}