//function syncWishlistStatus()
//{
//    $.ajax({
//        url: '/Wishlist/GetWishlistItems',
//        type: 'GET',
//        success: function (wishlistItems) {
//            $('.favorite').each(function () {
//                var $heartIcon = $(this);
//                var productId = $heartIcon.data('id');
//                if (wishlistItems.includes(productId)) {
//                    $heartIcon.addClass('active');
//                } else {
//                    $heartIcon.removeClass('active');
//                }
//            });
//        },
//        error: function () {
//            console.log('Error fetching wishlist items');
//        }
//    });
//}

//function handleWishlistResponse(response, $heartIcon, isAdding) {
//    if (response.success) {
//        if (isAdding) {
//            $heartIcon.addClass('active');
//        } else {
//            $heartIcon.removeClass('active');
//        }
//        alert(response.message);
//        // Sync wishlist status across all pages
//        syncWishlistStatus();
//    } else {
//        if (response.requireLogin) {
//            alert('Vui lòng đăng nhập để thêm sản phẩm vào danh sách yêu thích.');
//            // Có thể thêm code để chuyển hướng đến trang đăng nhập ở đây
//        } else {
//            alert(response.message || 'Có lỗi xảy ra. Vui lòng thử lại.');
//        }
//    }
//}

//$(document).ready(function () {
//    syncWishlistStatus();
//    //handleWishlistResponse();
//    // Existing click event handler
//    $('.favorite').click(function (e) {
//        e.preventDefault();
//        var productId = $(this).data('id');
//        var $heartIcon = $(this);

//        if ($heartIcon.hasClass('active')) {
//            // Xóa khỏi danh sách yêu thích
//            $.ajax({
//                url: '/Wishlist/RemoveFromWishlist',
//                type: 'POST',
//                data: { productId: productId },
//                success: function (response) {
//                    handleWishlistResponse(response, $heartIcon, false);
//                },
//                error: function () {
//                    alert('Đã xảy ra lỗi. Vui lòng thử lại sau.');
//                }
//            });
//        } else {
//            // Thêm vào danh sách yêu thích
//            $.ajax({
//                url: '/Wishlist/AddToWishlist',
//                type: 'POST',
//                data: { productId: productId },
//                success: function (response) {
//                    handleWishlistResponse(response, $heartIcon, true);
//                },
//                error: function () {
//                    alert('Đã xảy ra lỗi. Vui lòng thử lại sau.');
//                }
//            });
//        }
//    });


//});