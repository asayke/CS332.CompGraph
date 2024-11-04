import tkinter as tk


class PolygonApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Polygon Intersection App")

        # Настройка переменных
        self.canvas = tk.Canvas(root, width=600, height=600, bg='white')
        self.canvas.pack()
        self.polygons = [[], []]  # Два списка для точек каждого полигона
        self.current_poly_index = 0  # Индекс текущего полигона
        self.intersection_poly = []  # Пересечение полигонов

        # Добавление кнопок для управления
        self.btn_switch = tk.Button(root, text="Switch Polygon", command=self.switch_polygon)
        self.btn_switch.pack(side=tk.LEFT, padx=5, pady=5)

        self.btn_calculate = tk.Button(root, text="Calculate Intersection", command=self.calculate_intersection)
        self.btn_calculate.pack(side=tk.LEFT, padx=5, pady=5)

        self.btn_clear = tk.Button(root, text="Clear", command=self.clear_canvas)
        self.btn_clear.pack(side=tk.LEFT, padx=5, pady=5)

        # Обработка кликов мыши
        self.canvas.bind("<Button-1>", self.add_point)

    def add_point(self, event):
        """Добавляет точку в текущий полигон по щелчку мыши."""
        x, y = event.x, event.y
        self.polygons[self.current_poly_index].append((x, y))
        self.draw_polygon()

    def switch_polygon(self):
        """Переключает текущий полигон для добавления точек."""
        if self.current_poly_index == 0:
            self.current_poly_index = 1
        else:
            self.current_poly_index = 0

    def calculate_intersection(self):
        """Вычисляет пересечение полигонов и отображает его."""
        if len(self.polygons[0]) < 3 or len(self.polygons[1]) < 3:
            print("Оба полигона должны содержать как минимум 3 вершины.")
            return

        # Вычисляем пересечение с использованием алгоритма Сазерленда-Ходжмана
        self.intersection_poly = self.sutherland_hodgman(self.polygons[0], self.polygons[1])
        self.draw_intersection()

    def sutherland_hodgman(self, subject_polygon, clip_polygon):
        """Реализует алгоритм Сазерленда-Ходжмана для нахождения пересечения двух выпуклых полигонов."""
        output_list = subject_polygon
        for i in range(len(clip_polygon)):
            input_list = output_list
            output_list = []
            if len(input_list) == 0:
                break

            # Определяем текущую грань отсечения
            A = clip_polygon[i]
            B = clip_polygon[(i + 1) % len(clip_polygon)]

            for j in range(len(input_list)):
                # Определяем текущую и предыдущую вершину
                P = input_list[j]
                Q = input_list[(j + 1) % len(input_list)]

                # Проверяем положение точек относительно отрезка AB
                if self.is_inside(A, B, Q):
                    if not self.is_inside(A, B, P):
                        intersection_point = self.compute_intersection(A, B, P, Q)
                        output_list.append(intersection_point)
                    output_list.append(Q)
                elif self.is_inside(A, B, P):
                    intersection_point = self.compute_intersection(A, B, P, Q)
                    output_list.append(intersection_point)

        return output_list

    def is_inside(self, A, B, P):
        """Определяет, находится ли точка P слева от отрезка AB."""
        return (B[0] - A[0]) * (P[1] - A[1]) - (B[1] - A[1]) * (P[0] - A[0]) >= 0

    def compute_intersection(self, A, B, P, Q):
        """Вычисляет точку пересечения отрезков AB и PQ."""
        dx1, dy1 = B[0] - A[0], B[1] - A[1]
        dx2, dy2 = Q[0] - P[0], Q[1] - P[1]
        denom = dx1 * dy2 - dy1 * dx2

        if denom == 0:
            return P  # Отрезки параллельны, возвращаем одну из точек

        t = ((P[0] - A[0]) * dy2 - (P[1] - A[1]) * dx2) / denom
        intersection_point = (A[0] + t * dx1, A[1] + t * dy1)
        return intersection_point

    def draw_polygon(self):
        """Рисует текущий полигон и точки на холсте."""
        self.canvas.delete("poly")

        # Рисуем первый полигон
        if self.polygons[0]:
            self.canvas.create_polygon(self.polygons[0], outline='blue', fill='', width=2, tags="poly")
            for x, y in self.polygons[0]:
                self.canvas.create_oval(x - 3, y - 3, x + 3, y + 3, fill='blue', tags="poly")

        # Рисуем второй полигон
        if self.polygons[1]:
            self.canvas.create_polygon(self.polygons[1], outline='green', fill='', width=2, tags="poly")
            for x, y in self.polygons[1]:
                self.canvas.create_oval(x - 3, y - 3, x + 3, y + 3, fill='green', tags="poly")

    def draw_intersection(self):
        """Рисует область пересечения полигонов."""
        if self.intersection_poly:
            self.canvas.delete("intersection")
            self.canvas.create_polygon(self.intersection_poly, outline='red', fill='red', stipple="gray50", width=2,
                                       tags="intersection")

    def clear_canvas(self):
        """Очищает холст и сбрасывает все данные."""
        self.canvas.delete("all")
        self.polygons = [[], []]
        self.current_poly_index = 0
        self.intersection_poly = []


# Создаем окно приложения
root = tk.Tk()
app = PolygonApp(root)
root.mainloop()
