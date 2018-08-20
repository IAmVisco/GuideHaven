var index = 0, visual_index = 0, multiWidget;

function recreateSteps(step, header, content, images, raw_header, raw_content, imgs) {
    document.getElementById("step_holder").insertAdjacentHTML(
        'beforeend', '<div id="step' + (index + 1) + '" style="position:relative">'
        + '<hr/><label id="step_count_' + index + '" class="control-label" style="display: block">' + step + ' ' + (visual_index + 1) + '</label>'
        + '<label class="control-label" for="GuideSteps_' + index + '__Header">' + header + '</label>'
        + '<input type="text" id="GuideSteps_' + index + '__Header" name="GuideSteps[' + index + '].Header" class="form-control">'
        + '<label class="control-label" for="GuideSteps_' + index + '__Content">' + content + '</label>'

        + '<textarea id="DummyArea' + index + '" class="bs-textarea mdhtmlform-md" data-mdhtmlform-group="' + index + '" rows="5">'
        + toMarkdown(he.decode(raw_content)) + '</textarea>'
        + '<textarea id="GuideSteps_' + index + '__Content" name="GuideSteps[' + index + '].Content" '
        + 'class="mdhtmlform-html" data-mdhtmlform-group="' + index + '" style="display: none"></textarea>'

        + '<label class="control-label" style="margin-right: 3px;">' + images + '</label>'
        + '<input id="images' + index + '" hidden name="GuideSteps[' + index + '].Images" />'
        + '<input type="hidden" id="multi' + index + '" name="content" data-images-only data-multiple />'

        + '<button type="button" class="btn-link delete-btn" value="Delete" onclick="deleteStep(' + (index + 1) + ')">'
        + '<span class="glyphicon glyphicon-remove"></span></button></div >'
    );
    document.getElementById('GuideSteps_' + index + '__Header').value = he.decode(raw_header);
    multiWidget = uploadcare.MultipleWidget($("#multi" + index));
    multiWidget.onChange(function (info) {
        if (!info) widgetCleared(index);
    });
    multiWidget.onUploadComplete(function (info) {
        uploadHandler(info, index);
    });
    multiWidget.value(imgs);
    index++;
    visual_index++;
}