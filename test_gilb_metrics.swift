let x = 5
let y = 10
let z = 15
var result = 0

if x < y {
    result = result + 1
    if z > x {
        result = result + 2
        if y == 10 {
            result = result + 3
        } else if z != 20 {
            result = result + 4
       }
    }
    var innerCounter = 0
    while innerCounter < 2 {
        result = result + 1
        innerCounter += 1
    }
} else if x > z {
    result = result - 1
} else {
    result = result * 2
}

switch x {
case 1:
    result = result + 10
case 5:
    result = result + 20
    switch y {
    case 10:
        result = result + 30
        if z < 20 {
            result = result + 40
        }
    case 15:
        result = result + 50
    default:
        result = result + 60
    }
case 10:
    result = result + 70
default:
    result = result + 80
    if result > 100 {
        result = result - 50
    }
}

let final = result

var counter = 0
while counter < 3 {
    result = result + 5
    counter += 1
}

for i in 1...3 {
    switch i {
    case 1:
        result = result + 100
    case 2:
        result = result + 200
        for j in 1...2 {
            result = result + j * 10
        }
    case 3:
        result = result + 300
    default:
        result = result + 400
    }
}

for a in 1...2 {
    for b in 1...3 {
        result = result + a * b
        if a == b {
            result = result + 50
            if b > 1 {
                result = result + 25
                for c in 1...2 {
                    result = result + c
                }
            }
        } else if a < b {
            result = result + 10
        }
    }
}

var w = 0
for u in 1...3 {
    switch u {
    case 1:
        result = result + 1
        if result > 0 {
            for v in 1...2 {
                result = result + v
            }
        }
    case 2:
        result = result + 2
        for w2 in 1...2 {
            if w2 == 1 {
                result = result + 100
            } else if w2 == 2 {
                result = result + 200
                if result < 500 {
                    result = result + 50
                }
            }
        }
    default:
        result = result + 3
        if result < 500 {
            result = result + 50
            if result < 550 {
                result = result + 67
                if result < 700 {
                     result = result + 4
                }
            }
        }
    }
}

