$(function () {

    if ($("a.confirmDeletion").length) {
        $("a.confirmDeletion").click(() => {
            if (!confirm("Confirm deletion")) return false;
        });
    }

    if ($("div.alert.notification").length) {
        setTimeout(() => {
            $("div.alert.notification").fadeOut();
        }, 2000);
    }

});

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imgpreview').attr('src', e.target.result);
            $('#imgpreview').css('display', 'block');
            $('#imgpreview').css('max-width', '200px'); // Giới hạn kích thước
        };
        reader.readAsDataURL(input.files[0]);
    }
}