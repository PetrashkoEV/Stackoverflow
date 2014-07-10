$('.redaction').click(function () {
    var id = $(this).attr("id");
    var type = $(this).attr("title");

    var displayDiv = $(".newComment[id='" + type + "-" + id + "']").css('display');
    if (displayDiv == "block")
        $(".newComment[id='" + type + "-" + id + "']").hide("slow");
    else
        $(".newComment[id='" + type + "-" + id + "']").show("slow");
});

$('.newComment .button.black').click(function () {
    var id = $(this).attr("id");
    var type = $(this).attr("title");
    var body = $("textarea[id = '" + type + "-" + id + "']").val();

    if (body != "") {
        $.ajax({
            type: "POST",
            url: "/Question/AddComment",
            data: "id=" + id + "&type=" + type + "&body=" + body,
            success: function (msg) {
                parsingJsonComments(msg);
                $("textarea[id = '" + type + "-" + id + "']").val(""); // clear textarea
                $(".newComment[id = '" + type + "-" + id + "']").hide("slow"); // hide block newComment
            },
            error: function (xhr, ajaxOptions, errorThrown) {
                alert(xhr.responseText);
            }
        });
    }
});

function parsingJsonComments(msg) {
    var obj = jQuery.parseJSON(msg);

    var block = $(".comments[id='" + obj.Type + "-" + obj.Id + "']");
    var content = block.html();

    var commentDate = new Date(obj.Comment.PublicationDate);
    dateFormat.masks.hammerTime = "d mmmm yyyy 'at' H:mm";

    content = content + '<div class="comment">' + obj.Comment.Body +
                            ' - ' + obj.Comment.User.DisplayName +
                            ', commented in ' + commentDate.format("hammerTime") + '</div>';
    block.html(content);
}

$('.selectionBest, .best').click(function () {
    var id = $(this).attr("id");
    $.ajax({
        type: "POST",
        url: "/Question/Usefulness",
        data: "id=" + id,
        success: function () {
            $(".best").removeClass("best").addClass("selectionBest");
            $(".selectionBest[id='" + id + "']").removeClass("selectionBest").addClass("best");
        },
        error: function (xhr, ajaxOptions, errorThrown) {
            alert(xhr.responseText);
        }
    });
});