$('.voteUp').click(function () {
    var id = $(this).attr("id");
    var type = $(this).attr("title");

    $.ajax({
        type: "POST",
        url: "/Question/VoteUp",
        data: "id=" + id + "&type=" + type,
        success: function (msg) {
            var obj = jQuery.parseJSON(msg);
            $(".voteCount[id='" + type + "-" + id + "']").html(obj.AmountLike);
        },
        error: function (xhr, ajaxOptions, errorThrown) {
            alert(xhr.responseText);
        }
    });
});

$('.voteDown').click(function () {
    var id = $(this).attr("id");
    var type = $(this).attr("title");

    $.ajax({
        type: "POST",
        url: "/Question/VoteDown",
        data: "id=" + id + "&type=" + type,
        success: function (msg) {
            var obj = jQuery.parseJSON(msg);
            $(".voteCount[id='" + type + "-" + id + "']").html(obj.AmountLike);
        },
        error: function (xhr, ajaxOptions, errorThrown) {
            alert(xhr.responseText);
        }
    });

});