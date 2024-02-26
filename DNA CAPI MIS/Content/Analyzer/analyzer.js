$(document).ready(function () {
    var updateOutput = function (e) {
        var list = e.length ? e : $(e.target),
            output = list.data('output');
        if (output != null) {
            if (window.JSON) {
                output.val(window.JSON.stringify(list.nestable('serialize')));//, null, 2));
            } else {
                output.val('JSON browser support required for this demo.');
            }
        }
    };

    $('#pfAll').nestable({
        group: 1
    })
    //.on('change', updateOutput);

    $('#pfTop').nestable({
        group: 1
    })
    .on('change', updateOutput);

    $('#pfSide').nestable({
        group: 1
    })
    .on('change', updateOutput);

    // output initial serialised data
    updateOutput($('#pfTop').data('output', $('#pfTop-output')));
    updateOutput($('#pfAll').data('output', $('#pfAll-output')));
    updateOutput($('#pfSide').data('output', $('#pfSide-output')));

    $('#pfTop-menu').on('click', function (e) {
        var target = $(e.target),
            action = target.data('action');
        if (action === 'expand-all') {
            $('.dd').nestable('expandAll');
        }
        if (action === 'collapse-all') {
            $('.dd').nestable('collapseAll');
        }
    });

    
});

