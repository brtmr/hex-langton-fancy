class LangtonApp
    #canvas for drawing, button for starting, input for running.
    constructor: ( @canvas, @button, @input ) ->
        @width  = canvas.width
        @height = canvas.height
        @rules  = []
        @input.value = 'RL'
        @start = @execute.bind this
        @stop  = @halt.bind this
        @button.addEventListener('click', @start)
        @hex =
            width: 7
            height: 2.0/Math.sqrt(3) * 7
        @context = @canvas.getContext('2d');
        @running = false
        @colors = [
                '#cc001b'
                '#2c00a6'
                '#7a9900'
                '#ff8800'
                '#cc335c'
                '#ff40f2'
                '#33cc99'
                '#394d13'
                '#442d59'
                '#1a332e'
                '#331a1a'
                '#e6ace2'
                '#608071'
            ]
        # the amount of hexagons we can fit along the x-axis:
        @j_max = (@width/@hex.width) - 2
        # the amount of hexagons we can fit along the y-axis:
        @i_max = (@height/(3.0/4.0 * @hex.height)) - 1

    execute: ->
        if (not @running) and @parse_input()
            @button.innerHTML = 'stop'
            #todo: remove and add event listeners
            @button.removeEventListener('click', @start)
            @button.addEventListener('click', @stop)
            @running = true
            @ant =
                direction : 0
                i         : Math.floor(@i_max/2)
                j         : Math.floor(@j_max/2)
            @board = []
            for i in [0..@i_max]
                row = []
                for j in [0..@j_max]
                    row[j]=0
                @board[i] = row
            @loop()

    halt: -> 
        @running = false
        @button.removeEventListener('click', @stop)
        @button.addEventListener('click', @start)
        @button.innerHTML = 'start'

    loop: ->
        if @running
            if @ant.j >= @j_max || @ant.j<0 || @ant.i >= @i_max || @ant.i < 0
                @halt()
                return
            @canvas.width = @canvas.width
            @draw_board()
            @draw_ant()
            @turn_and_step()
            window.requestAnimationFrame(@loop.bind this)
        else 
            return

    parse_input: ->
        @rules = []
        input_string = @input.value
        for char in input_string.toUpperCase()
            switch(char)
                when 'R' then @rules.push 1
                when 'S' then @rules.push 2
                when 'B' then @rules.push 3
                when 'P' then @rules.push -2
                when 'L' then @rules.push -1
                when 'F' then @rules.push 0
                else 
                    alert 'Rule contains invalid character.'
                    @rules = []
                    return false
        return true

    ## logic methods ##
    turn_and_step: -> 
        #what color is the field the ant is standing on?
        color = @board[@ant.i][@ant.j]
        #what rule does apply?
        dir = @rules[color]
        #cycle the tile
        @board[@ant.i][@ant.j] = (color+1) % (@rules.length)
        @turn(dir)
        @step()

    turn: (dir) ->
        @ant.direction += dir
        if @ant.direction > 5 
            @ant.direction -= 6
        if @ant.direction < 0
            @ant.direction += 6

    step: ->
        if @ant.direction == 1
            @ant.j = @ant.j + 1
            return
        if @ant.direction == 4
            @ant.j = @ant.j - 1
            return
        # are we in an even row?
        if @ant.i%2==0
            #top right
            if @ant.direction == 0
                @ant.i = @ant.i - 1
            #bottom right
            if @ant.direction == 2
                @ant.i = @ant.i + 1
            #bottom left
            if @ant.direction == 3
                @ant.i = @ant.i + 1
                @ant.j = @ant.j - 1
            #top left
            if @ant.direction == 5
                @ant.i = @ant.i - 1
                @ant.j = @ant.j - 1
        else
            #top right
            if @ant.direction == 0
                @ant.i = @ant.i - 1
                @ant.j = @ant.j + 1
            #bottom right
            if @ant.direction == 2
                @ant.i = @ant.i + 1
                @ant.j = @ant.j + 1
            #bottom left
            if @ant.direction == 3
                @ant.i = @ant.i + 1
            #top left
            if @ant.direction == 5
                @ant.i = @ant.i - 1

    ## drawing methods ##

    mk_coordinates: (i,j) ->
        if (i%2==0)
            x=(@hex.width/2.0) + j*@hex.width
        else
            x=(j+1)*@hex.width
        y=(@hex.height/2.0) + i*(@hex.height*(3/4))
        return [x,y]

    draw_ant: ->
        cos = @mk_coordinates(@ant.i,@ant.j)
        @context.beginPath()
        @context.arc(cos[0], cos[1], 2, 0, 2 * Math.PI, false);
        @context.fillStyle = 'red'
        @context.fill()

    draw_board: ->
        for i in [0..@i_max]
            for j in [0..@j_max]
                @drawSingleHex(i,j)

    drawSingleHex: (i,j) ->
        cos = @mk_coordinates(i,j)
        x = cos[0]
        y = cos[1]
        if @board[i][j]
            cos = @mk_coordinates(i,j)
            @context.beginPath()
            @context.moveTo(x-0.5*@hex.width, y-0.25*@hex.height)
            @context.lineTo(x               , y- 0.5*@hex.height)
            @context.lineTo(x+0.5*@hex.width, y-0.25*@hex.height)
            @context.lineTo(x+0.5*@hex.width, y+0.25*@hex.height)
            @context.lineTo(x               , y+ 0.5*@hex.height)
            @context.lineTo(x-0.5*@hex.width, y+0.25*@hex.height)
            @context.lineTo(x-0.5*@hex.width, y-0.25*@hex.height)

            @context.lineWidth = 1
            @context.strokeStyle = '#aaa'
            @context.stroke()
            @context.fillStyle = @colors[(@board[i][j]-1) % (@colors.length)]
            @context.fill()

window.onload = () ->
    canvas = document.getElementById('langton')
    button = document.getElementById('run')
    input = document.getElementById('sequence')
    app = new LangtonApp(canvas, button, input)
