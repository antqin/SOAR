import pandas as pd 
import numpy as np 
import matplotlib.pyplot as plt 
import seaborn as sns


df = pd.read_csv("figma_prototypes_quant_feedback.csv")
df = df.query("option != 'option4'")
df = df.assign(
	option = lambda x : np.where(x["option"] == "option1", "carpet path", "arrow")
)

df_grp = df.assign(n=1).groupby(["response", "option"])["n"].sum().reset_index()
df_totals = df_grp.groupby(["option"])["n"].sum().reset_index().rename(columns = {"n":"total_n"})
df_grp = df_grp.merge(
	df_totals, on = "option", how = "left"
)
df_grp["pct"] = 100 * df_grp["n"] / df_grp["total_n"]


g = sns.catplot(data = df_grp, x = "response", y = "pct", hue = "option", kind = "bar", order = ["Yes", "It is okay", "No"])
g.set_axis_labels("Do you like Option X?", "Percent")
plt.show()

# option1 = df['option1'].value_counts(normalize=True)
# option2 = df['option2'].value_counts(normalize=True)
# option3 = df['option3'].value_counts(normalize=True)
# option4 = df['option4'].value_counts(normalize=True)

# fig, ax = plt.subplots(1, 1)
# ax.bar(option1.index, option1, label = "Option 1", color = "#1f77b4")
# ax.bar(option2.index, option2, label = "Option 2", color = "#ff7f0e")
# ax.bar(option3.index, option3, label = "Option 3", color = "#2ca02c")
# ax.bar(option4.index, option4, label = "Option 4", color = "#d62728")
# fig.legend()
# plt.show()
