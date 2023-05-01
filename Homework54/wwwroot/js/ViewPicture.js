$(() => {
    const pictureId = $('#like-button').data('picture-id');
    setInterval(() => {
        $.get(`/home/getLikesCount?pictureId=${pictureId}`, function (likesCount) {
            $('#likes-label').text(`${likesCount} Likes`)
        });
    }, 1000)



    $('#like-button').on('click', () => {
        $.post(`/home/addLike?pictureId=${pictureId}`, function () {
            $('#like-button').prop('disabled', true);
        });
    });
})